using System;
using System.Threading.Tasks;
using BlogApp.Common.Contracts.Accessors;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BlogApp.Accessors
{
    public sealed class BlogCacheAccessor : ICacheAccessor
    {
        #region Constructor and private members
        private readonly IConfiguration _config;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public BlogCacheAccessor(IConfiguration config)
        {
            _config = config
                ?? throw new ArgumentNullException(nameof(config));

            _connectionMultiplexer = ConnectionMultiplexer.Connect(_config["redis:endpoint"]);
        }

        internal BlogCacheAccessor(IConfiguration config, IConnectionMultiplexer multiplexer)
        {
            _config = config
                ?? throw new ArgumentNullException(nameof(config));

            _connectionMultiplexer = multiplexer
                ?? throw new ArgumentNullException(nameof(multiplexer));
        }
        #endregion

        public async Task<TEntity> GetEnt<TEntity>(string key) where TEntity : class
        {
            var db = _connectionMultiplexer.GetDatabase();
            var text = await db.StringGetAsync(key);

            return string.IsNullOrEmpty(text) 
                ? null 
                : JsonConvert.DeserializeObject<TEntity>(text);
        }

        public async Task CacheEnt<TEntity>(string key, TEntity ent, TimeSpan? ttl = null)
        {
            if (ttl == null)
                ttl = TimeSpan.FromMilliseconds(double.Parse(_config["redis:ttlMsDefault"]));

            var db = _connectionMultiplexer.GetDatabase();
            await db.StringSetAsync(key, JsonConvert.SerializeObject(ent), ttl, When.Always, CommandFlags.FireAndForget);
        }
    }
}
