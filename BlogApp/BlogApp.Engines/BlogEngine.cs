using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Common.Contracts.Accessors;
using BlogApp.Common.Contracts.Engines;
using BlogApp.Common.Models;

namespace BlogApp.Engines
{
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public sealed class BlogEngine : IBlogEngine
    {
        #region Constructor and private members
        private readonly IBlogAccessor _dbAccessor;
        private readonly ICacheAccessor _cacheAccessor;

        public BlogEngine(IBlogAccessor dbAccessor, ICacheAccessor cacheAccessor)
        {
            _dbAccessor = dbAccessor
                ?? throw new ArgumentNullException(nameof(dbAccessor));

            _cacheAccessor = cacheAccessor
                ?? throw new ArgumentNullException(nameof(cacheAccessor));
        }
        #endregion

        public async Task<IEnumerable<PostHeader>> GetPageOfHeaders(int pageNumber, int countPerPage)
        {
            var cacheKey = $"Headers::{countPerPage}::{pageNumber}";
            var cached = await _cacheAccessor.GetEnt<IEnumerable<PostHeader>>(cacheKey);

            if (cached != null && cached.Any())
                return cached;

            var headers = await _dbAccessor.GetPostHeadersByPage(pageNumber, countPerPage);
            if(headers != null && headers.Any())
                await _cacheAccessor.CacheEnt(cacheKey, headers);

            return headers;
        }
    }
}
