using System.Collections.Generic;
using System.Threading.Tasks;
using BlogApp.Common.Models;

namespace BlogApp.Common.Contracts.Engines
{
    public interface IBlogEngine
    {
        Task<IEnumerable<MetaTag>> GetTagList();

        Task<IEnumerable<PostHeader>> GetPageOfHeaders(int pageNumber, int countPerPage);

        Task<IEnumerable<PostHeader>> GetPageOfHeadersByTag(int pageNumber, int countPerPage, string tag);
    }
}
