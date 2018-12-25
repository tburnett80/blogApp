using System;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Accessors.EF;
using BlogApp.Accessors.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogApp.Accessors.Tests
{
    [TestClass]
    public class EfMappingTests
    {
        private static DbContextOptions _opts;
        private static string _testTag;
        private static string _testTitle;
        private static string _testBody;

        [ClassInitialize]
        public static void Init(TestContext tctx)
        {
            _opts = new DbContextOptionsBuilder()
                .UseNpgsql($"Server={TestConstants.Server};Port=5432;Database={Guid.NewGuid().ToString().Replace("-", "")};User Id=user1;Password=password1;")
                .Options;

            _testTag = $"{Guid.NewGuid().ToString().Replace("-", "")}";
            _testTitle = $"{Guid.NewGuid().ToString().Replace("-", "")}";
            _testBody = $"{Guid.NewGuid().ToString().Replace("-", "")}";

            using (var ctx = new BlogContext(_opts))
            {
                ctx.Database.EnsureCreated();

                var tag1 = new TagEntity { Text = _testTag };

                //Add a tag
                ctx.Tags.Add(tag1);
                ctx.SaveChanges();

                var body = new PostBodyEntity { Markdown = _testBody };

                //add body
                ctx.Bodies.Add(body);
                ctx.SaveChanges();

                var header = new PostHeaderEntity
                {
                    BodyId = body.Id,
                    Title = _testTitle
                };

                //add header
                ctx.Headers.Add(header);
                ctx.SaveChanges();

                var join = new PostTagEntity
                {
                    TagId = tag1.Id,
                    PostId = header.Id
                };

                ctx.PostTags.Add(join);
                ctx.SaveChanges();
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
                //Get a full post by searching for a tag
                var results = await ctx.Headers
                    .Include(e => e.Body)
                    .Include(e => e.PostTags)
                    .ThenInclude(e => e.Tag)
                    .Where(e => e.PostTags.Any(t => t.Tag.Text.Equals(_testTag)))
                    .ToListAsync();

                var result = results.FirstOrDefault();

                //Top level result collection checks
                Assert.IsNotNull(results, "Should contain a post header ent as collection");
                Assert.AreEqual(1, results.Count, "Should be a single post");

                //Post header checks
                Assert.IsNotNull(result, "Should contain a post header ent");
                Assert.IsInstanceOfType(result, typeof(PostHeaderEntity));
                Assert.AreEqual(1, result.Id, "Only header in db so id should be 1");
                Assert.AreEqual(_testTitle, result.Title, "Should be equal");
                Assert.IsNotNull(result.Body, "Should contain the body");
                Assert.AreEqual(1, result.PostTags.Count, "Should be a single tag");
                Assert.IsTrue(result.PostTags.All(pt => pt.Tag != null), "Should contain the tag");

                //Body Checks
                var body = result.Body;
                Assert.IsNotNull(body, "Should not be null");
                Assert.IsInstanceOfType(body, typeof(PostBodyEntity));
                Assert.AreEqual(1, body.Id, "Only body in db so id should be 1");
                Assert.AreEqual(_testBody, body.Markdown, "Should be equal");

                //Tag Checks
                var tag = result.PostTags.FirstOrDefault()?.Tag;
                Assert.IsNotNull(tag, "Should not be null");
                Assert.IsInstanceOfType(tag, typeof(TagEntity));
                Assert.AreEqual(1, tag.Id, "Only tag in db so id should be 1");
                Assert.AreEqual(_testTag, tag.Text, "Should be equal");
            }
            
        }
    }
}
