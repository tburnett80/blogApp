using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Common.Contracts.Accessors;
using BlogApp.Common.Contracts.Engines;
using BlogApp.Common.Models;
using BlogApp.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BlogApp.Unit.Tests
{
    [TestClass]
    public class BlogPostManagerTests
    {
        private static IConfiguration _cfg;

        [ClassInitialize]
        public static void Init(TestContext tctx)
        {
            _cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("PageSize", "2")
                }).Build();
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetPageOfHeaders()
        {
            //Arrange
            var mockDb = new Mock<IBlogAccessor>();
            var mockEngine = new Mock<IBlogEngine>();

            mockEngine.Setup(m => m.GetPageOfHeaders(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => new[]
                {
                    new PostHeader
                    {
                        BodyId = 1,
                        Id = 1,
                        Title = TestConstants.GuidString,
                        Tags = new[]
                        {
                            new Tag
                            {
                                Id = 1,
                                Text = TestConstants.GuidString
                            },
                            new Tag
                            {
                                Id = 2,
                                Text = TestConstants.GuidString
                            }
                        }
                    },
                    new PostHeader
                    {
                        BodyId = 2,
                        Id = 2,
                        Title = TestConstants.GuidString,
                        Tags = new[]
                        {
                            new Tag
                            {
                                Id = 3,
                                Text = TestConstants.GuidString
                            },
                            new Tag
                            {
                                Id = 4,
                                Text = TestConstants.GuidString
                            }
                        }
                    },
                });

            var manager = new BlogPostManager(mockEngine.Object, mockDb.Object, _cfg);

            //Act
            var results = await manager.GetPageOfHeaders(0);

            //Assert
            Assert.IsNotNull(results, "Should never be null.");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<PostHeader>));
            Assert.AreEqual(2, results.Count(), "Should return two items");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetPageOfTaggedHeaders()
        {
            //Arrange
            var mockDb = new Mock<IBlogAccessor>();
            var mockEngine = new Mock<IBlogEngine>();

            mockEngine.Setup(m => m.GetPageOfHeadersByTag(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(() => new[]
                {
                    new PostHeader
                    {
                        BodyId = 1,
                        Id = 1,
                        Title = TestConstants.GuidString,
                        Tags = new[]
                        {
                            new Tag
                            {
                                Id = 1,
                                Text = TestConstants.GuidString
                            },
                            new Tag
                            {
                                Id = 2,
                                Text = TestConstants.GuidString
                            }
                        }
                    },
                    new PostHeader
                    {
                        BodyId = 2,
                        Id = 2,
                        Title = TestConstants.GuidString,
                        Tags = new[]
                        {
                            new Tag
                            {
                                Id = 3,
                                Text = TestConstants.GuidString
                            },
                            new Tag
                            {
                                Id = 4,
                                Text = TestConstants.GuidString
                            }
                        }
                    },
                });

            var manager = new BlogPostManager(mockEngine.Object, mockDb.Object, _cfg);

            //Act
            var results = await manager.GetTaggedPageOfHeaders(0, TestConstants.GuidString);

            //Assert
            Assert.IsNotNull(results, "Should never be null.");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<PostHeader>));
            Assert.AreEqual(2, results.Count(), "Should return two items");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetTagList()
        {
            //Arrange
            var mockDb = new Mock<IBlogAccessor>();
            var mockEngine = new Mock<IBlogEngine>();

            mockEngine.Setup(m => m.GetTagList())
                .ReturnsAsync(() => new[]
                {
                    new MetaTag { Count = 3, Tag = TestConstants.GuidString }, 
                    new MetaTag { Count = 1, Tag = TestConstants.GuidString },
                    new MetaTag { Count = 2, Tag = TestConstants.GuidString },
                });

            var manager = new BlogPostManager(mockEngine.Object, mockDb.Object, _cfg);

            //Act
            var results = await manager.GetTagList();

            //Assert
            Assert.IsNotNull(results, "Should never be null.");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<MetaTag>));
            Assert.AreEqual(3, results.Count(), "Should return three items");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetBodyById()
        {
            //Arrange
            var mockDb = new Mock<IBlogAccessor>();
            var mockEngine = new Mock<IBlogEngine>();

            mockDb.Setup(m => m.GetPostById(It.IsAny<int>()))
                .ReturnsAsync(() => new Post
                {
                    Body = TestConstants.GuidString,
                    Header = new PostHeader
                    {
                        BodyId = 12,
                        Id = 12,
                        Title = TestConstants.GuidString,
                        Tags = new []
                        {
                            new Tag { Id = 4, Text = TestConstants.GuidString },
                            new Tag { Id = 6, Text = TestConstants.GuidString }
                        }
                    }
                });

            var manager = new BlogPostManager(mockEngine.Object, mockDb.Object, _cfg);

            //Act
            var results = await manager.GetPostBodyById(12);

            //Assert
            Assert.IsNotNull(results, "Should never be null.");
            Assert.IsInstanceOfType(results, typeof(Post));
        }
    }
}
