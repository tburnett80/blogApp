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
        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task EnumerateBucketsTest()
        {
            //Arrange
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("minio:endPoint","192.168.1.1:9000"),
                    new KeyValuePair<string, string>("minio:accessKey","abc"),
                    new KeyValuePair<string, string>("minio:secretKey","abc123def"),
                }).Build();

            var accessor = new BlogBlobAccessor(cfg);

            //Act
            var buckets = await accessor.ListBuckets();

            //Assert
            Assert.IsNotNull(buckets, "Should always be a collection even when empty");
            Assert.IsInstanceOfType(buckets, typeof(IEnumerable<string>));
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task AddblobTest1()
        {
            //Arrange
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("minio:endPoint","192.168.1.1:9000"),
                    new KeyValuePair<string, string>("minio:accessKey","abc"),
                    new KeyValuePair<string, string>("minio:secretKey","abc123def"),
                }).Build();

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
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("minio:endPoint","192.168.1.1:9000"),
                    new KeyValuePair<string, string>("minio:accessKey","abc"),
                    new KeyValuePair<string, string>("minio:secretKey","abc123def"),
                }).Build();

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
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("minio:endPoint","192.168.1.1:9000"),
                    new KeyValuePair<string, string>("minio:accessKey","abc"),
                    new KeyValuePair<string, string>("minio:secretKey","abc123def"),
                }).Build();

            var accessor = new BlogBlobAccessor(cfg);

            //Act
            var blob = await accessor.GetBlob("testblob3", "test2.txt");
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
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("minio:endPoint","192.168.1.1:9000"),
                    new KeyValuePair<string, string>("minio:accessKey","abc"),
                    new KeyValuePair<string, string>("minio:secretKey","abc123def"),
                }).Build();

            var accessor = new BlogBlobAccessor(cfg);

            //Act
            await accessor.DeleteBucket("testblob2");

            //Assert
            //Assert.IsNotNull(blob, "Should always be an array even when empty");
            //Assert.IsInstanceOfType(blob, typeof(byte[]));
        }
    }
}
