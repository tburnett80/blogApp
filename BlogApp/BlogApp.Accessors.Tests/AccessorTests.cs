using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogApp.Accessors.Tests
{
    [TestClass]
    public class AccessorTests
    {
        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task GetPostHeaders()
        {
            //10.200.7.50
            var opts = new DbContextOptionsBuilder()
                .UseNpgsql("Server=10.200.7.50;Port=5432;Database=blog;User Id=user1;Password=password1;")
                .Options;

            var accessor = new BlogAccessor(opts);

            var heads = await accessor.GetPostHeadersByPage();

            Assert.IsNotNull(heads, "Should not be null");
        }
    }
}
