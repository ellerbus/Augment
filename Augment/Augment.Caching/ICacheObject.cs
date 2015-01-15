using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augment.Caching
{
    /// <summary>
    /// Represents a cached object.
    /// </summary>
    /// <typeparam name="T">Type of the cached object</typeparam>
    public interface ICacheObject<T> where T : class
    {
        /// <summary>
        /// Adds new keys that identify the cached object.
        /// </summary>
        /// <param name="cacheKeys">The cache keys</param>
        /// <returns>Fluent interface for further configuring the cache</returns>
        ICacheObject<T> By(params object[] cacheKeys);

        /// <summary>
        /// Specifies the duration and duration type of the cache.
        /// </summary>
        /// <param name="cacheDuration">Duration before cache expires</param>
        /// <param name="cacheExpiration">Duration rule before cache expires</param>
        /// <param name="cachePriority">Priority for cache removal</param>
        /// <returns>Fluent interface for further configuring the cache</returns>
        ICacheObject<T> DurationOf(TimeSpan cacheDuration, CacheExpiration cacheExpiration = CacheExpiration.Absolute, CachePriority cachePriority = CachePriority.Normal);

        /// <summary>
        /// Retrieves the cached object. If found from cache (by the key) then the cached object is returned.
        /// Otherwise, the cachedObjectLoader is called to load the object, and it is then added to cache.
        /// </summary>
        T CachedObject { get; }
    }
}