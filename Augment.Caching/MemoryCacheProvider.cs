using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Augment.Caching
{
    /// <summary>
    /// Implemented for HttpRuntime Cache Provider
    /// </summary>
    public class MemoryCacheProvider : ICacheProvider
    {
        #region ICacheProvider Members

        private static object _lock = new object();

        private DateTimeOffset NoExpiration = MemoryCache.InfiniteAbsoluteExpiration;
        private TimeSpan NoSliding = MemoryCache.NoSlidingExpiration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <param name="expires"></param>
        /// <param name="priority"></param>
        public void Add(string key, object value, TimeSpan duration, CacheExpiration expires, CachePriority priority)
        {
            CacheItem ci = new CacheItem(key, value);

            CacheItemPriority pic = CacheItemPriority.Default;

            if (priority == CachePriority.NotRemovable)
            {
                pic = CacheItemPriority.NotRemovable;
            }

            switch (expires)
            {
                case CacheExpiration.Absolute:

                    CacheItemPolicy pa = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow.Add(duration),
                        Priority = pic
                    };

                    MemoryCache.Default.Add(ci, pa);

                    break;

                case CacheExpiration.Sliding:

                    CacheItemPolicy ps = new CacheItemPolicy
                    {
                        SlidingExpiration = duration,
                        Priority = pic
                    };

                    MemoryCache.Default.Add(ci, ps);

                    break;

                default:
                    throw new InvalidOperationException("Unknown Cache Expiration " + expires);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            return MemoryCache.Default.Get(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Remove(string key)
        {
            return MemoryCache.Default.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllKeys()
        {
            lock (_lock)
            {
                List<string> keys = MemoryCache.Default.Select(x => x.Key).ToList();

                return keys;
            }
        }

        #endregion
    }
}
