using System;
using System.Web;
using System.Web.Caching;
using AB.Data;

namespace AzureDashboard.Services
{
    /// <summary>
    /// Implementation for System.Web.Caching
    /// </summary>
    /// <remarks>
    /// Create a new instance has no cost.
    /// </remarks>
    public class WebCache : ICacheProvider
    {
        private readonly object Mutex = new object();

        public T Get<T>(string id)
        {
            if (id == null)
            {
                return default(T);
            }
            lock (Mutex)
            {
                string cacheKey = CreateCacheKey<T>(id);
                return (T)HttpRuntime.Cache.Get(cacheKey);
            }
        }

        private string CreateCacheKey<T>(string id)
        {
            return string.Format("{0}_{1}", id, typeof(T).Name);
        }

        public T Put<T>(string id, T value, TimeSpan relativeExpiration, CacheItemPriority priority = CacheItemPriority.Default, CacheItemRemovedCallback callback = null)
        {
            if (id == null)
            {
                return default(T);
            }
            lock (Mutex)
            {
                string cacheKey = CreateCacheKey<T>(id);
                if (callback == null)
                {
                    HttpRuntime.Cache.Insert(cacheKey, value, null, DateTime.UtcNow.Add(relativeExpiration), Cache.NoSlidingExpiration);
                }
                else
                {
                    HttpRuntime.Cache.Insert(cacheKey, value, null, DateTime.UtcNow.Add(relativeExpiration), Cache.NoSlidingExpiration, priority, callback);   
                }
                return value;
            }
        }

        public T PutWithSliding<T>(string id, T value, TimeSpan slidingExpiration, CacheItemPriority priority = CacheItemPriority.Default, CacheItemRemovedCallback callback = null)
        {
            if (id == null)
            {
                return default(T);
            }
            lock (Mutex)
            {
                string cacheKey = CreateCacheKey<T>(id);
                if (callback == null)
                {
                    HttpRuntime.Cache.Insert(cacheKey, value, null, Cache.NoAbsoluteExpiration, slidingExpiration);
                }
                else
                {
                    HttpRuntime.Cache.Insert(cacheKey, value, null, Cache.NoAbsoluteExpiration, slidingExpiration, priority, callback);
                }
                return value;
            }
        }

        public void Evict<T>(string id)
        {
            if (id == null)
            {
                return;
            }
            lock (Mutex)
            {
                string cacheKey = CreateCacheKey<T>(id);
                HttpRuntime.Cache.Remove(cacheKey);
            }
        }
    }
}
