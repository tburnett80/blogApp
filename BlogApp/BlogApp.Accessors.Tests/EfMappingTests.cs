using System;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Accessors.EF;
using BlogApp.Accessors.Entities;
using BlogApp.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogApp.Accessors.Tests
{
    [TestClass]
    public class EfMappingTests
    {
        private static DbContextOptions _opts;

        [ClassInitialize]
        public static void Init(TestContext tctx)
        {
            _opts = new DbContextOptionsBuilder()
                .UseNpgsql($"Server={TestConstants.Server};Port=5432;Database={Guid.NewGuid().ToString().Replace("-", "")};User Id=user1;Password=password1;")
                .Options;

            using (var ctx = new BlogContext(_opts))
            {
                ctx.Database.EnsureCreated();

                //Add some tags

                //
            }
        }

        [ClassCleanup]
        public static void TearDown()
        {
            using (var ctx = new BlogContext(_opts))
            {
                ctx.Database.EnsureDeleted();
            }
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task TestBlogMappings()
        {
            using (var ctx = new BlogContext(_opts))
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
