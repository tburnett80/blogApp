using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Accessors.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogApp.Accessors.Tests
{
    [TestClass]
    public class EfMappingTests
    {
        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task TestBlogMappings()
        {
            var opts = new DbContextOptionsBuilder();
            opts.UseNpgsql("Server=192.168.1.1;Port=5432;Database=blog;User Id=user1;Password=password1;");

            using (var ctx = new BlogContext(opts.Options))
            {
                //var tags = await ctx.Tags.ToListAsync();
                //var bodies = await ctx.Bodies.ToListAsync();
                //var headers = await ctx.Headers.Include(e => e.Body).ToListAsync();
                var stuff = await ctx.PostTags
                    .Include(e => e.Tag)
                    .Include(e => e.Post)
                    .ThenInclude(e => e.Body)
                    .ToListAsync();

                int x = 0;
                
            }
            
        }
    }
}
