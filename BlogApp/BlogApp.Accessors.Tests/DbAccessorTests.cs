using System.Linq;
using System.Threading.Tasks;
using BlogApp.Accessors.EF;
using BlogApp.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogApp.Accessors.Tests
{
    [TestClass]
    public class DbAccessorTests
    {
        private static DbContextOptions _opts;

        [ClassInitialize]
        public static void Init(TestContext tctx)
        {
            _opts = new DbContextOptionsBuilder()
                .UseNpgsql($"Server={TestConstants.Server};Port=5432;Database={TestConstants.GuidString};User Id=user1;Password=password1;")
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
        public async Task AddAllNewPostTest()
        {
            //Arrange
            var post = new Post
            {
                Body = "This is a sample post body.",
                Header = new PostHeader
                {
                    Title = "Test Post",
                    Tags = new[]
                    {
                        new Tag
                        {
                            Text = "Test1"
                        },
                        new Tag
                        {
                            Text = "Test2"
                        },
                    }
                }
            };

            var accessor = new BlogAccessor(_opts);

            //Act
            var result = await accessor.AddPost(post);

            //Assert
            Assert.IsNotNull(result, "Should never be null");
            Assert.IsTrue(result > 2, "Should have added header and body and two tags");
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task AddPostExistingTagTest()
        {
            //Arrange
            var post = new Post
            {
                Body = "This is a sample post body.",
                Header = new PostHeader
                {
                    Title = "Test Post2",
                    Tags = new[]
                    {
                        new Tag
                        {
                            Text = "Test3"
                        },
                        new Tag
                        {
                            Text = "Test4"
                        },
                    }
                }
            };

            var accessor = new BlogAccessor(_opts);
            await accessor.AddTags(new[] { new Tag { Text = "Test3" } });

            //Act
            var result = await accessor.AddPost(post);

            //Assert
            Assert.IsNotNull(result, "Should never be null");
            Assert.IsTrue(result > 2, "Should have added header and body and two tags");
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task GetPostCountTest()
        {
            //Arrange
            var post = new Post
            {
                Body = TestConstants.GuidString,
                Header = new PostHeader
                {
                    Title = TestConstants.GuidString,
                    Tags = new[]
                    {
                        new Tag
                        {
                            Text = TestConstants.GuidString
                        },
                        new Tag
                        {
                            Text = TestConstants.GuidString
                        },
                    }
                }
            };

            var accessor = new BlogAccessor(_opts);
            await accessor.AddPost(post);

            //Act
            var result = await accessor.GetPostCount();

            //Assert
            Assert.IsNotNull(result, "Should never be null");
            Assert.IsTrue(result > 0, "Should at minimum have the post we just added");
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task GetPagedHeadersTest()
        {
            //Arrange
            var accessor = new BlogAccessor(_opts);
            for (var ndx = 0; ndx < 5; ndx++)
            {
                await accessor.AddPost(new Post
                {
                    Body = TestConstants.GuidString,
                    Header = new PostHeader
                    {
                        Title = TestConstants.GuidString,
                        Tags = new[]
                        {
                            new Tag
                            {
                                Text = TestConstants.GuidString
                            },
                            new Tag
                            {
                                Text = TestConstants.GuidString
                            },
                        }
                    }
                });
            }

            //Act
            var result = await accessor.GetPostHeadersByPage(1, 2);

            //Assert
            Assert.IsNotNull(result, "Should never be null");
            Assert.AreEqual(2, result.Count(), "Should contain 2 post headers from the second page");
        }
    }
}
