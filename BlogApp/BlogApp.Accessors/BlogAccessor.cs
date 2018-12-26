﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;
using BlogApp.Accessors.EF;
using BlogApp.Accessors.ModelConverters;
using BlogApp.Common.Contracts.Accessors;
using BlogApp.Common.Models;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("BlogApp.Accessors.Tests")]
namespace BlogApp.Accessors
{
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public sealed class BlogAccessor : IBlogAccessor
    {
        #region Constructor and Private Members
        private readonly DbContextOptions _opt;

        public BlogAccessor(DbContextOptions opts)
        {
            _opt = opts
                ?? throw new ArgumentNullException(nameof(opts));
        }
        #endregion

        public async Task<int> GetPostCount()
        {
            using (var ctx = new BlogContext(_opt))
            {
                return await ctx.Headers.CountAsync();
            }
        }

        public async Task<IEnumerable<PostHeader>> GetPostHeadersByPage(int pageNumber = 0, int pageSize = 10)
        {
            using (var ctx = new BlogContext(_opt))
            {
                var headers = await ctx.Headers
                    .Include(e => e.PostTags)
                    .ThenInclude(e => e.Tag)
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return headers.Convert();
            }
        }

        public async Task<int> AddPost(Post dto)
        {
            using (var ctx = new BlogContext(_opt))
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var ent = dto.Convert();
                    var tags = await AddTags(dto.Header.Tags);

                    await ctx.Headers.AddAsync(ent);
                    var cnt = await ctx.SaveChangesAsync();

                    var postTagJoins = ent.Convert(tags);

                    await ctx.PostTags.AddRangeAsync(postTagJoins);
                    cnt += await ctx.SaveChangesAsync();

                    scope.Complete();
                    return cnt;
                }
                catch (Exception)
                {
                    //TODO log exception
                    return -1;
                }
            }
        }

        public async Task<IEnumerable<Tag>> AddTags(IEnumerable<Tag> tags)
        {
            using (var ctx = new BlogContext(_opt))
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var tagText = tags.Select(t => t.Text);
                    var existingTags = await ctx.Tags
                        .Where(t => tagText.Contains(t.Text))
                        .ToListAsync();

                    var newTags = tags.Where(t => !existingTags.Any(et => et.Text.Equals(t.Text)))
                        .Convert()
                        .ToList();

                    if (newTags.Any())
                    {
                        await ctx.Tags.AddRangeAsync(newTags);
                        await ctx.SaveChangesAsync();
                    }

                    scope.Complete();
                    return existingTags
                        .Concat(newTags)
                        .Convert();
                }
                catch (Exception ex)
                {
                    //TODO: log exception
                    return new Tag[0];
                }
            }
        }
    }
}

