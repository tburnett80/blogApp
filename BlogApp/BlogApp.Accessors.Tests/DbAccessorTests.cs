using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
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

        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task GetPagedHeadersByTagTest()
        {
            //Arrange
            var accessor = new BlogAccessor(_opts);

            var tagText = TestConstants.GuidString;
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
                                Text = tagText
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
            var result = await accessor.GetPostHeaderPageByTag(1, 2, tagText);

            //Assert
            Assert.IsNotNull(result, "Should never be null");
            Assert.AreEqual(2, result.Count(), "Should contain 2 post headers from the second page matching the tag");
            Assert.IsTrue(result.All(r => r.Tags.Any(t => t.Text.Equals(tagText))));
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task GetPostTest()
        {
            //Arrange
            var bodyId = -1;
            var bodyText = TestConstants.GuidString;
            using (var ctx = new BlogContext(_opts))
            {
                var tags = new []
                {
                    new PostTagEntity{ Tag = new TagEntity { Text = TestConstants.GuidString }},
                    new PostTagEntity{ Tag = new TagEntity { Text = TestConstants.GuidString }}
                };
                var body = new PostBodyEntity { Markdown = bodyText };
                var header = new PostHeaderEntity { Title = TestConstants.GuidString, Body = body };
                foreach (var pt in tags)
                    pt.Post = header;
                
                await ctx.Headers.AddAsync(header);
                await ctx.SaveChangesAsync();

                bodyId = body.Id;
            }

            var accessor = new BlogAccessor(_opts);

            //Act
            var result = await accessor.GetPostById(bodyId);

            //Assert
            Assert.IsTrue(bodyId > 0, "Should be at least 1");
            Assert.IsNotNull(result, "Should never be null");
            Assert.IsInstanceOfType(result, typeof(Post));
            Assert.AreEqual(bodyText, result.Body, "Should contain the test body text");
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task GetMetaTagsTest()
        {
            //Arrange
            var tagText = TestConstants.GuidString;

            using (var ctx = new BlogContext(_opts))
            {
                var tag = new TagEntity { Text = tagText };
                var header = new PostHeaderEntity { Title = TestConstants.GuidString, Body = new PostBodyEntity { Markdown = TestConstants.GuidString }};
                var header2 = new PostHeaderEntity { Title = TestConstants.GuidString, Body = new PostBodyEntity { Markdown = TestConstants.GuidString }};
                await ctx.Tags.AddAsync(tag);
                await ctx.Headers.AddAsync(header);
                await ctx.Headers.AddAsync(header2);
                await ctx.SaveChangesAsync();

                await ctx.PostTags.AddAsync(new PostTagEntity {PostId = header.Id, TagId = tag.Id});
                await ctx.PostTags.AddAsync(new PostTagEntity {PostId = header2.Id, TagId = tag.Id});
                await ctx.SaveChangesAsync();
            }

            var accessor = new BlogAccessor(_opts);

            //Act
            var results = await accessor.GetTagList();
            var match = results.FirstOrDefault(t => t.Tag.Equals(tagText));

            //Assert
            Assert.IsNotNull(results, "Should never be null");
            Assert.IsInstanceOfType(results, typeof(IEnumerable<MetaTag>));
            Assert.IsTrue(results.Any());
            Assert.IsNotNull(match, "Should be at least one match");
            Assert.IsInstanceOfType(match, typeof(MetaTag));
            Assert.AreEqual(2, match.Count, "Should be equal");
            Assert.AreEqual(tagText, match.Tag, "Should be equal");
        }
    }
}
