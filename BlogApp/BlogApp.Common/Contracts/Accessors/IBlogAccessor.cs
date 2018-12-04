using System.Collections.Generic;
using System.Threading.Tasks;
using BlogApp.Common.Models;

namespace BlogApp.Common.Contracts.Accessors
{
    public interface IBlogAccessor
    {
        /// <summary>
        /// Gets count of posts
        /// </summary>
        /// <returns></returns>
        Task<int> GetPostCount();

        /// <summary>
        /// Get Paged results of post headers
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        /// <param name="pageSize">Number of items in a page</param>
        /// <returns></returns>
        Task<IEnumerable<PostHeader>> GetPostHeadersByPage(int pageNumber = 0, int pageSize = 10);
    }
}
