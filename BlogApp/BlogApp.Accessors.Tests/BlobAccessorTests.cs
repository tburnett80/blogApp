using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogApp.Accessors.Tests
{
    [TestClass]
    public class BlobAccessorTests
    {
        private static Dictionary<string, string> _blobs;
        private static IConfiguration cfg;

        [ClassInitialize]
        public static void init(TestContext ctx)
        {
            _blobs = new Dictionary<string, string>();

            BlobAccessorTests.cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("minio:endPoint",$"{TestConstants.Server}:9000"),
                    new KeyValuePair<string, string>("minio:accessKey","abc"),
                    new KeyValuePair<string, string>("minio:secretKey","abc123def"),
                }).Build();

            var accessor = new BlogBlobAccessor(cfg);

            //Setup data for 'GetblobTest' to be downloaded
            _blobs.Add("GetblobTest", $"{Guid.NewGuid()}");
            _blobs.Add("DeleteBucketTest", $"{Guid.NewGuid()}");

            for (var ndx = 0; ndx < 3; ndx++)
                accessor.AddBlob(_blobs["GetblobTest"], $"test{ndx}.txt", generateTestBlob()).Wait();

            for (var ndx = 0; ndx < 5; ndx++)
                accessor.AddBlob(_blobs["DeleteBucketTest"], $"{Guid.NewGuid()}", generateTestBlob()).Wait();
        }

        [ClassCleanup]
        public static void cleanup()
        {
            var accessor = new BlogBlobAccessor(cfg);
            var buckets = accessor.ListBuckets().Result;
            foreach (var bucket in buckets)
                accessor.DeleteBucket(bucket).Wait();
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task EnumerateBucketsTest()
        {
            //Arrange
            var accessor = new BlogBlobAccessor(cfg);

            //Act
            var buckets = await accessor.ListBuckets();

            //Assert
            Assert.IsNotNull(buckets, "Should always be a collection even when empty");
            Assert.IsInstanceOfType(buckets, typeof(IEnumerable<string>));
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task EnumerateBucketTest()
        {
            //Arrange
            var accessor = new BlogBlobAccessor(cfg);

            //Act
            var bucketItems = await accessor.EnumerateBucket("testblob3");

            //Assert
            Assert.IsNotNull(bucketItems, "Should always be a collection even when empty");
            Assert.IsInstanceOfType(bucketItems, typeof(IEnumerable<string>));
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task AddblobTest1()
        {
            //Arrange
            var blob = new MemoryStream();
            using (var sr = new StreamWriter(blob, Encoding.ASCII, 1024, true))
            {
                sr.WriteLine("this is a test line.");
                sr.WriteLine("this is a second test line.");

                sr.Flush();
            }
            blob.Position = 0;

            var accessor = new BlogBlobAccessor(cfg);

            //Act
            await accessor.AddBlob("testblob2", "test.txt", blob, blob.Length, "text/plain");

            //Assert
            //Assert.IsNotNull(buckets, "Should always be a collection even when empty");
            //Assert.IsInstanceOfType(buckets, typeof(IEnumerable<string>));
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task AddblobTest2()
        {
            //Arrange
            var blob = Encoding.ASCII.GetBytes("This is my second test.\r\nDoes it work?\r\n");
            var accessor = new BlogBlobAccessor(cfg);

            //Act
            await accessor.AddBlob("testblob3", "test2.txt", blob, "text/plain");

            //Assert
            //Assert.IsNotNull(buckets, "Should always be a collection even when empty");
            //Assert.IsInstanceOfType(buckets, typeof(IEnumerable<string>));
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task GetblobTest()
        {
            //Arrange
            var accessor = new BlogBlobAccessor(cfg);

            //Act
            var blob = await accessor.GetBlob(_blobs["GetblobTest"], "test1.txt");
            var text = Encoding.ASCII.GetString(blob);

            //Assert
            Assert.IsNotNull(blob, "Should always be an array even when empty");
            Assert.IsInstanceOfType(blob, typeof(byte[]));
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task DeleteBucketTest()
        {
            //Arrange
            var accessor = new BlogBlobAccessor(cfg);

            //Act
            var result = await accessor.DeleteBucket(_blobs["DeleteBucketTest"]);

            //Assert
            Assert.IsNotNull(result, "Should always be a bool");
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue(result, "Should be successful");
        }

        private static byte[] generateTestBlob()
        {
            var blob = new MemoryStream();
            using (var sr = new StreamWriter(blob, Encoding.ASCII, 1024, true))
            {
                for (var ndx = 0; ndx < 3; ndx++)
                    sr.WriteLine($"lkjkj{Guid.NewGuid()}asdfasdf");
                sr.Flush();
            }

            blob.Position = 0;
            return blob.ToArray();
        }
    }
}
