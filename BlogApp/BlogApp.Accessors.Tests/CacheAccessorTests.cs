using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;

namespace BlogApp.Accessors.Tests
{
    [TestClass]
    public class CacheAccessorTests
    {
        private static IConfiguration _cfg;
        private static IConnectionMultiplexer _connectionMultiplexer;
        private static Dictionary<string, string> _testKeys;

        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            _cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("redis:endpoint",$"{TestConstants.Server}:6379"),
                    new KeyValuePair<string, string>("redis:ttlMsDefault","7200000")
                }).Build();

            _connectionMultiplexer = ConnectionMultiplexer.Connect(_cfg["redis:endpoint"]);
            _testKeys = new Dictionary<string, string>();
            
            _testKeys.Add("GetCachedItemTest", $"{Guid.NewGuid()}");
            _testKeys.Add("GetCachedItemNotCachedTest", $"{Guid.NewGuid()}");
            _testKeys.Add("GetCachedItemExpiredTest", $"{Guid.NewGuid()}");
            _testKeys.Add("SetCachedItemTest", $"{Guid.NewGuid()}");

            var accessor = new BlogCacheAccessor(_cfg, _connectionMultiplexer);
            accessor.CacheEnt(_testKeys["GetCachedItemTest"], new CacheTestModel
            {
                Number = 25,
                Text = $"{_testKeys["GetCachedItemTest"]}",
                StringCollection = new[]
                {
                    "Test 1",
                    "Test 2"
                }
            }).Wait();

            accessor.CacheEnt(_testKeys["GetCachedItemExpiredTest"], new CacheTestModel
            {
                Number = 17,
                Text = $"{_testKeys["GetCachedItemExpiredTest"]}",
                StringCollection = new[]
                {
                    "Test 3",
                    "Test 4"
                }
            }, TimeSpan.FromMilliseconds(50)).Wait();
        }

        [ClassCleanup]
        public static void Teardown()
        {
            var db = _connectionMultiplexer.GetDatabase();
            foreach (var key in _testKeys.Values)
                db.KeyDelete(key);
            
            _connectionMultiplexer?.Dispose();
        }

        internal class CacheTestModel
        {
            public CacheTestModel()
            {
                StringCollection = new string[0];
            }

            public string Text { get; set; }

            public int Number { get; set; }

            public IEnumerable<string> StringCollection { get; set; }
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task GetCachedItemTest()
        {
            //Arrange
            var accessor = new BlogCacheAccessor(_cfg, _connectionMultiplexer);

            //Act
            var result = await accessor.GetEnt<CacheTestModel>(_testKeys["GetCachedItemTest"]);

            //Assert
            Assert.IsNotNull(result, "Should be an object from cache");
            Assert.IsInstanceOfType(result, typeof(CacheTestModel));
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task GetCachedItemNotCachedTest()
        {
            //Arrange
            var accessor = new BlogCacheAccessor(_cfg, _connectionMultiplexer);

            //Act
            var result = await accessor.GetEnt<CacheTestModel>(_testKeys["GetCachedItemNotCachedTest"]);

            //Assert
            Assert.IsNull(result, "Should be nothing cached on this key");
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task GetCachedItemExpiredTest()
        {
            //Arrange
            var accessor = new BlogCacheAccessor(_cfg, _connectionMultiplexer);

            //Act
            Task.Delay(500).Wait();
            var result = await accessor.GetEnt<CacheTestModel>(_testKeys["GetCachedItemExpiredTest"]);

            //Assert
            Assert.IsNull(result, "Should be nothing cached on this key");
        }

        [TestCategory("Integration Test")]
        [TestMethod]
        public async Task SetCachedItemTest()
        {
            //Arrange
            var accessor = new BlogCacheAccessor(_cfg, _connectionMultiplexer);
            var item = new CacheTestModel
            {
                Number = 64,
                Text = $"{_testKeys["SetCachedItemTest"]}",
                StringCollection = new[]
                {
                    "Last test first",
                    "this should work"
                }
            };

            //Act
            await accessor.CacheEnt(_testKeys["SetCachedItemTest"], item);
            Task.Delay(500).Wait();
            var result = await accessor.GetEnt<CacheTestModel>(_testKeys["SetCachedItemTest"]);

            //Assert
            Assert.IsNotNull(result, "Should have cached the item, and retrieved it");
            Assert.IsInstanceOfType(result, typeof(CacheTestModel));
            Assert.AreEqual(item.Number, result.Number, "Should be equal");
            Assert.AreEqual(item.Text, result.Text, "Should be equal");
            Assert.AreEqual(item.StringCollection.Count(), result.StringCollection.Count(), "Should be equal");
        }
    }
}
