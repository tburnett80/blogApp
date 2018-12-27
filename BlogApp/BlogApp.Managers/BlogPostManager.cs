using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogApp.Common.Contracts.Accessors;
using BlogApp.Common.Contracts.Engines;
using BlogApp.Common.Contracts.Managers;
using BlogApp.Common.Models;
using Microsoft.Extensions.Configuration;

namespace BlogApp.Managers
{
    public sealed class BlogPostManager : IBlogPostManager
    {
        #region Constructor and Private members
        private readonly IBlogEngine _blogEngine;
        private readonly IBlogAccessor _dbAccessor;
        private readonly IConfiguration _config;

        public BlogPostManager(IBlogEngine blogEngine, IBlogAccessor dbAccessor, IConfiguration config)
        {
            _blogEngine = blogEngine
                ?? throw new ArgumentNullException(nameof(blogEngine));

            _dbAccessor = dbAccessor
                ?? throw new ArgumentNullException(nameof(dbAccessor));

            _config = config
                ?? throw new ArgumentNullException(nameof(config));
        }
        #endregion

        public async Task<IEnumerable<MetaTag>> GetTagList()
        {
            return await _blogEngine.GetTagList();
        }

        public async Task<IEnumerable<PostHeader>> GetPageOfHeaders(int pageNum)
        {
            return await _blogEngine.GetPageOfHeaders(pageNum, int.Parse(_config["PageSize"]));
        }

        public async Task<IEnumerable<PostHeader>> GetTaggedPageOfHeaders(int pageNum, string tag)
        {
            return await _blogEngine.GetPageOfHeadersByTag(pageNum, int.Parse(_config["PageSize"]), tag);
        }

        public async Task<Post> GetPostBodyById(int bodyId)
        {
            return await _dbAccessor.GetPostById(bodyId);
        }
    }
}
