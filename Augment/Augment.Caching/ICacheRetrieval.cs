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
    public interface ICacheRetrieval<T> where T : class
    {
        /// <summary>
        /// Adds new keys that identify the cached object.
        /// </summary>
        /// <param name="cacheKeys">The cache keys</param>
        /// <returns>Fluent interface for further configuring the cache</returns>
        ICacheRetrieval<T> By(params object[] cacheKeys);

        /// <summary>
        /// Removes the cached object. If found from cache (by the key) then the cached object is returned.
        /// Otherwise, null is returned
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// Removes the cached object. If found from cache (by the key) then the cached object is returned.
        /// Otherwise, null is returned
        /// </summary>
        T Remove();

        /// <summary>
        /// Retrieves the cached object. If found from cache (by the key) then the cached object is returned.
        /// Otherwise, the cachedObjectLoader is called to load the object, and it is then added to cache.
        /// </summary>
        T CachedObject { get; }
    }
}