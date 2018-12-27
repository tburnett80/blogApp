using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Common.Contracts.Accessors;
using BlogApp.Common.Models;
using BlogApp.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BlogApp.Unit.Tests
{
    [TestClass]
    public class BlogEngineTests
    {
        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetHeadersPageNoDataTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockDb.Setup(m => m.GetPostHeadersByPage(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => new PostHeader[0]);

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetPageOfHeaders(0, 2);

            //Assert
            Assert.IsNotNull(results, "Should never be null");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<PostHeader>));
            Assert.IsFalse(results.Any(), "Should be no headers on this page");

            mockCache.Verify(m => 
                m.CacheEnt(It.IsAny<string>(), It.IsAny<IEnumerable<PostHeader>>(), It.IsAny<TimeSpan?>()), 
                Times.Never, "Should be skipped because of null");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetHeadersPageNullDataTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockDb.Setup(m => m.GetPostHeadersByPage(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetPageOfHeaders(0, 2);

            //Assert
            Assert.IsNull(results, "Should be null");

            mockCache.Verify(m =>
                m.CacheEnt(It.IsAny<string>(), It.IsAny<IEnumerable<PostHeader>>(), It.IsAny<TimeSpan?>()),
                Times.Never, "Should be skipped because of null");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetHeadersPageNoCacheTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockDb.Setup(m => m.GetPostHeadersByPage(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => new[]
                {
                    new PostHeader
                    {
                        BodyId = 1,
                        Id = 1,
                        Title = TestConstants.GuidString,
                        Tags = new []
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
                        Tags = new []
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

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetPageOfHeaders(0, 2);

            //Assert
            Assert.IsNotNull(results, "Should never be null");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<PostHeader>));
            Assert.AreEqual(2, results.Count(), "Should be two headers on this page");

            mockCache.Verify(m =>
                m.CacheEnt<IEnumerable<PostHeader>>(It.IsAny<string>(), It.IsAny<IEnumerable<PostHeader>>(), It.IsAny<TimeSpan?>()),
                Times.Once, "Should be called once to cache results");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetHeadersPageCachedTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockCache.Setup(m => m.GetEnt<IEnumerable<PostHeader>>(It.IsAny<string>()))
                .ReturnsAsync(() => new[]
                {
                    new PostHeader
                    {
                        BodyId = 1,
                        Id = 1,
                        Title = TestConstants.GuidString,
                        Tags = new []
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
                        Tags = new []
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

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetPageOfHeaders(0, 2);

            //Assert
            Assert.IsNotNull(results, "Should never be null");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<PostHeader>));
            Assert.AreEqual(2, results.Count(), "Should be two headers on this page");

            mockDb.Verify(m => m.GetPostHeadersByPage(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never, "Should never reach db because results were cached.");

            mockCache.Verify(m =>
                m.CacheEnt<IEnumerable<PostHeader>>(It.IsAny<string>(), It.IsAny<IEnumerable<PostHeader>>(), It.IsAny<TimeSpan?>()),
                Times.Never, "Should have bailed before getting here");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetHeadersPageTaggedNoDataTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockDb.Setup(m => m.GetPostHeaderPageByTag(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(() => new PostHeader[0]);

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetPageOfHeadersByTag(0, 2, TestConstants.GuidString);

            //Assert
            Assert.IsNotNull(results, "Should never be null");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<PostHeader>));
            Assert.IsFalse(results.Any(), "Should be no headers on this page");

            mockCache.Verify(m =>
                m.CacheEnt(It.IsAny<string>(), It.IsAny<IEnumerable<PostHeader>>(), It.IsAny<TimeSpan?>()),
                Times.Never, "Should be skipped because of null");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetHeadersPageTaggedNullDataTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockDb.Setup(m => m.GetPostHeaderPageByTag(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetPageOfHeadersByTag(0, 2, TestConstants.GuidString);

            //Assert
            Assert.IsNull(results, "Should be null");

            mockCache.Verify(m =>
                m.CacheEnt(It.IsAny<string>(), It.IsAny<IEnumerable<PostHeader>>(), It.IsAny<TimeSpan?>()),
                Times.Never, "Should be skipped because of null");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetHeadersPageTaggedNoCacheTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockDb.Setup(m => m.GetPostHeaderPageByTag(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(() => new[]
                {
                    new PostHeader
                    {
                        BodyId = 1,
                        Id = 1,
                        Title = TestConstants.GuidString,
                        Tags = new []
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
                        Tags = new []
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

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetPageOfHeadersByTag(0, 2, TestConstants.GuidString);

            //Assert
            Assert.IsNotNull(results, "Should never be null");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<PostHeader>));
            Assert.AreEqual(2, results.Count(), "Should be two headers on this page");

            mockCache.Verify(m =>
                m.CacheEnt<IEnumerable<PostHeader>>(It.IsAny<string>(), It.IsAny<IEnumerable<PostHeader>>(), It.IsAny<TimeSpan?>()),
                Times.Once, "Should be called once to cache results");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetHeadersPageTaggedCachedTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockCache.Setup(m => m.GetEnt<IEnumerable<PostHeader>>(It.IsAny<string>()))
                .ReturnsAsync(() => new[]
                {
                    new PostHeader
                    {
                        BodyId = 1,
                        Id = 1,
                        Title = TestConstants.GuidString,
                        Tags = new []
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
                        Tags = new []
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

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetPageOfHeadersByTag(0, 2, TestConstants.GuidString);

            //Assert
            Assert.IsNotNull(results, "Should never be null");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<PostHeader>));
            Assert.AreEqual(2, results.Count(), "Should be two headers on this page");

            mockDb.Verify(m => m.GetPostHeaderPageByTag(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()),
                Times.Never, "Should never reach db because results were cached.");

            mockCache.Verify(m =>
                m.CacheEnt<IEnumerable<PostHeader>>(It.IsAny<string>(), It.IsAny<IEnumerable<PostHeader>>(), It.IsAny<TimeSpan?>()),
                Times.Never, "Should have bailed before getting here");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetTagsNoDataTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockDb.Setup(m => m.GetTagList())
                .ReturnsAsync(() => new MetaTag[0]);

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetTagList();

            //Assert
            Assert.IsNotNull(results, "Should never be null");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<MetaTag>));
            Assert.IsFalse(results.Any(), "Should be no tags");

            mockCache.Verify(m => 
                m.CacheEnt(It.IsAny<string>(), It.IsAny<IEnumerable<MetaTag>>(), It.IsAny<TimeSpan?>()), 
                Times.Never, "Should be skipped because of null");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetTagsNullDataTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockDb.Setup(m => m.GetTagList())
                .ReturnsAsync(() => null);

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetTagList();

            //Assert
            Assert.IsNull(results, "Should be null");

            mockCache.Verify(m =>
                m.CacheEnt(It.IsAny<string>(), It.IsAny<IEnumerable<MetaTag>>(), It.IsAny<TimeSpan?>()),
                Times.Never, "Should be skipped because of null");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetTagsNoCacheTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockDb.Setup(m => m.GetTagList())
                .ReturnsAsync(() => new[]
                {
                    new MetaTag { Count = 3, Tag = TestConstants.GuidString }, 
                    new MetaTag { Count = 1, Tag = TestConstants.GuidString }, 
                    new MetaTag { Count = 2, Tag = TestConstants.GuidString }
                });

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetTagList();

            //Assert
            Assert.IsNotNull(results, "Should never be null");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<MetaTag>));
            Assert.AreEqual(3, results.Count(), "Should be three tags");

            mockCache.Verify(m =>
                m.CacheEnt<IEnumerable<MetaTag>>(It.IsAny<string>(), It.IsAny<IEnumerable<MetaTag>>(), It.IsAny<TimeSpan?>()),
                Times.Once, "Should be called once to cache results");
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public async Task GetTagsCachedTest()
        {
            //Arrange
            var mockCache = new Mock<ICacheAccessor>();
            var mockDb = new Mock<IBlogAccessor>();

            mockCache.Setup(m => m.GetEnt<IEnumerable<MetaTag>>(It.IsAny<string>()))
                .ReturnsAsync(() => new[]
                {
                    new MetaTag { Count = 3, Tag = TestConstants.GuidString }, 
                    new MetaTag { Count = 1, Tag = TestConstants.GuidString }, 
                    new MetaTag { Count = 2, Tag = TestConstants.GuidString }
                });

            var engine = new BlogEngine(mockDb.Object, mockCache.Object);

            //Act
            var results = await engine.GetTagList();

            //Assert
            Assert.IsNotNull(results, "Should never be null");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<MetaTag>));
            Assert.AreEqual(3, results.Count(), "Should be three tags");

            mockDb.Verify(m => m.GetTagList(),
                Times.Never, "Should never reach db because results were cached.");

            mockCache.Verify(m =>
                m.CacheEnt<IEnumerable<MetaTag>>(It.IsAny<string>(), It.IsAny<IEnumerable<MetaTag>>(), It.IsAny<TimeSpan?>()),
                Times.Never, "Should have bailed before getting here");
        }
    }
}
