using System;
using System.Collections.Generic;
using System.Linq;
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
        private static DbContextOptions opts;

        [ClassInitialize]
        public static void init(TestContext ctx)
        {
            var serverIp = "192.168.1.1";//"10.200.7.50";
            opts = new DbContextOptionsBuilder()
                .UseNpgsql($"Server={serverIp};Port=5432;Database=blog;User Id=user1;Password=password1;")
                .Options;
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task TestBlogMappings()
        {
            using (var ctx = new BlogContext(opts))
            {
                //var tags = await ctx.Tags.ToListAsync();
                //var bodies = await ctx.Bodies.ToListAsync();
                //var headers = await ctx.Headers.Include(e => e.Body).ToListAsync();

                //var headers = await ctx.Headers
                //    .Include(e => e.Body)
                //    .Include(e => e.PostTags)
                //    .ThenInclude(e => e.Tag)
                //    .ToListAsync();

                var headers = await ctx.Headers
                    .Include(e => e.Body)
                    .Include(e => e.PostTags)
                    .ThenInclude(e => e.Tag)
                    .Where(e => e.PostTags.Any(t => t.Tag.Text.Equals("test2"))) //Search for posts with this tag text
                    .ToListAsync();

                int x = 0;
                
            }
            
        }
    }
}
