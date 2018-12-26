using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Common.Models;

namespace BlogApp.Common.Contracts.Engines
{
    public interface IBlogEngine
    {
        Task<IEnumerable<PostHeader>> GetPageOfHeaders(int pageNumber, int countPerPage);
    }
}
