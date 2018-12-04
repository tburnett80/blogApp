using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BlogApp.Accessors.EF;
using BlogApp.Accessors.ModelConverters;
using BlogApp.Common.Contracts.Accessors;
using BlogApp.Common.Models;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("BlogApp.Accessors.Tests")]
namespace BlogApp.Accessors
{
    public sealed class BlogAccessor : IBlogAccessor
    {
        private readonly DbContextOptions _opt;

        public BlogAccessor(DbContextOptions opts)
        {
            _opt = opts
                ?? throw new ArgumentNullException(nameof(opts));
        }

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
    }
}

