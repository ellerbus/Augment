using System;

namespace Augment.Caching
{
    /// <summary>
    /// Represents an implementation for a cache manager (ie: HttpRuntime.Cache)
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// Cache results of <paramref name="cachedObjectLoader"/> into the implmented cache manager.
        /// </summary>
        /// <typeparam name="T">Type of the object being cached</typeparam>
        /// <param name="cachedObjectLoader">Code that loads the to-be-cached object</param>
        /// <returns>Implemented Interface for configuring the cache</returns>
        ICacheObject<T> Cache<T>(Func<T> cachedObjectLoader) where T : class;

        /// <summary>
        /// Finds the cached results.
        /// </summary>
        /// <typeparam name="T">Type of the object being cached</typeparam>
        /// <returns>Implemented Interface for configuring the cache</returns>
        ICacheRetrieval<T> Find<T>() where T : class;
    }
}