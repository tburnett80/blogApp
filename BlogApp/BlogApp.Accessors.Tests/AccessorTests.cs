using System;
using System.Threading.Tasks;
using BlogApp.Accessors.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogApp.Accessors.Tests
{
    [TestClass]
    public class AccessorTests
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
        public async Task GetPostHeaders()
        {
            var accessor = new BlogAccessor(_opts);
            var heads = await accessor.GetPostHeadersByPage();

            Assert.IsNotNull(heads, "Should not be null");
        }
    }
}
