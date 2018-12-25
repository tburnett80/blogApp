using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogApp.Accessors.Tests
{
    [TestClass]
    public class AccessorTests
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
        public async Task GetPostHeaders()
        {
            var accessor = new BlogAccessor(opts);
            var heads = await accessor.GetPostHeadersByPage();

            Assert.IsNotNull(heads, "Should not be null");
        }
    }
}
