using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augment.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Inserts a cache entry into the cache without overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="expirationTime">Absolute expiration value</param>
        /// <param name="expires">Cache Expiration logic</param>
        /// <param name="priority">Cache Removal Priority</param>
        void Add(string key, object value, TimeSpan expirationTime, CacheExpiration expires, CachePriority priority);

        /// <summary>
        /// Gets the cache value for the specified key
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <returns>The cache value for the specified key, if the entry exists; otherwise, <see langword="null"/>.</returns>
        object Get(string key);

        /// <summary>
        /// Removes a cache entry from the cache. 
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <returns>If the entry is found in the cache, the removed cache entry; otherwise, <see langword="null"/>.</returns>
        object Remove(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetAllKeys();
    }
}