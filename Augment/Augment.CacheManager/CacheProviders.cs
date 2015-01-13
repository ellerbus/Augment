using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace Augment.Cache
{
    /// <summary>
    /// Implemented for HttpRuntime Cache Provider
    /// </summary>
    public class HttpRuntimeCacheProvider : ICacheProvider
    {
        #region ICacheProvider Members

        private static object _lock = new object();

        private DateTime NoExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
        private TimeSpan NoSliding = System.Web.Caching.Cache.NoSlidingExpiration;

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
            switch (expires)
            {
                case CacheExpiration.Absolute:
                    HttpRuntime.Cache.Add(key, value, null, DateTime.UtcNow.Add(duration), NoSliding, (CacheItemPriority)priority, null);
                    break;
                case CacheExpiration.Sliding:
                    HttpRuntime.Cache.Add(key, value, null, NoExpiration, duration, (CacheItemPriority)priority, null);
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
            return HttpRuntime.Cache.Get(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Remove(string key)
        {
            return HttpRuntime.Cache.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllKeys()
        {
            lock (_lock)
            {
                IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();

                List<string> keys = new List<string>();

                while (enumerator.MoveNext())
                {
                    keys.Add(enumerator.Key as string);
                }

                return keys;
            }
        }

        #endregion
    }
}
