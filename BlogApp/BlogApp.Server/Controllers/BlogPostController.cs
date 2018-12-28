using System;
using System.Threading.Tasks;
using BlogApp.Common.Contracts.Managers;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Server.Controllers
{
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        #region constructor and private members
        private readonly IBlogPostManager _manager;

        public BlogPostController(IBlogPostManager manager)
        {
            _manager = manager
                ?? throw new ArgumentNullException(nameof(manager));
        }
        #endregion

        [Route("blog/tags/v1")]
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            return Ok(await _manager.GetTagList());
        }
    }
}