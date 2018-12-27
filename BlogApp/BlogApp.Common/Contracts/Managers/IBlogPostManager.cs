using System.Collections.Generic;
using System.Threading.Tasks;
using BlogApp.Common.Models;

namespace BlogApp.Common.Contracts.Managers
{
    public interface IBlogPostManager
    {
        Task<IEnumerable<PostHeader>> GetPageOfHeaders(int pageNum);
    }
}
