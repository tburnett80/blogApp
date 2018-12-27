using System.Collections.Generic;
using System.Threading.Tasks;
using BlogApp.Common.Models;

namespace BlogApp.Common.Contracts.Managers
{
    public interface IBlogPostManager
    {
        Task<IEnumerable<MetaTag>> GetTagList();

        Task<IEnumerable<PostHeader>> GetPageOfHeaders(int pageNum);

        Task<IEnumerable<PostHeader>> GetTaggedPageOfHeaders(int pageNum, string tag);

        Task<Post> GetPostBodyById(int bodyId);
    }
}
