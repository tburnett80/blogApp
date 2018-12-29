using System.Collections.Generic;
using System.Threading.Tasks;
using BlogApp.Common.Models;

namespace BlogApp.Common.Contracts.Accessors
{
    public interface IBlogAccessor
    {
        Task EnsureCreated();

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
        Task<IEnumerable<PostHeader>> GetPostHeadersByPage(int pageNumber, int pageSize);

        /// <summary>
        /// Get Paged results of post headers filtered by a tag
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve</param>
        /// <param name="pageSize">Number of items in a page</param>
        /// <param name="tag"></param>
        /// <returns></returns>
        Task<IEnumerable<PostHeader>> GetPostHeaderPageByTag(int pageNumber, int pageSize, string tag);

        /// <summary>
        /// Add an entire post object from Editor
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> AddPost(Post dto);

        /// <summary>
        /// Will add tags that do not already exist
        /// And lookup tags that match and do
        /// then return a merged list
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        Task<IEnumerable<Tag>> AddTags(IEnumerable<Tag> tags);

        /// <summary>
        /// Will retrieve the body for a header by id
        /// </summary>
        /// <param name="bodyId"></param>
        /// <returns></returns>
        Task<Post> GetPostById(int bodyId);

        /// <summary>
        /// Gets list of tags and the post counts for each
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MetaTag>> GetTagList();
    }
}
