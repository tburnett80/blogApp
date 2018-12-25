using System;
using System.Threading.Tasks;

namespace BlogApp.Common.Contracts.Accessors
{
    public interface ICacheAccessor
    {
        Task<TEntity> GetEnt<TEntity>(string key) where TEntity: class;

        Task CacheEnt<TEntity>(string key, TEntity ent, TimeSpan? ttl = null);
    }
}
