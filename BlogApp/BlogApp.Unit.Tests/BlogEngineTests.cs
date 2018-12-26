using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task GetHeaderPageNoCacheTest()
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
        }
    }
}
