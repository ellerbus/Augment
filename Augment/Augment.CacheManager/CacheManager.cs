﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//  http://www.sharepointblues.com/2010/09/03/custom-caching-done-fluently/
namespace Augment.Cache
{
    #region Enums

    /// <summary>
    /// Specifies the relative priority of items stored in the System.Web.Caching.Cache
    /// object.
    /// </summary>
    public enum CacheExpiration
    {
        /// <summary>
        /// Absolute Cache Duration
        /// </summary>
        Absolute,
        /// <summary>
        /// Sliding Cache Duration
        /// </summary>
        Sliding
    }

    /// <summary>
    /// Specifies the relative priority of items stored in the System.Web.Caching.Cache
    /// object.
    /// </summary>
    public enum CachePriority
    {
        /// <summary>
        /// Cache items with this priority level are the most likely to be deleted from
        /// the cache as the server frees system memory.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Cache items with this priority level are more likely to be deleted from the
        /// cache as the server frees system memory than items assigned a System.Web.Caching.CacheItemPriority.Normal
        /// priority.
        /// </summary>
        BelowNormal = 2,

        /// <summary>
        /// Cache items with this priority level are likely to be deleted from the cache
        /// as the server frees system memory only after those items with System.Web.Caching.CacheItemPriority.Low
        /// or System.Web.Caching.CacheItemPriority.BelowNormal priority. This is the
        /// default.
        /// </summary>
        Normal = 3,

        /// <summary>
        /// The default value for a cached item's priority is System.Web.Caching.CacheItemPriority.Normal.
        /// </summary>
        Default = 3,

        /// <summary>
        /// Cache items with this priority level are less likely to be deleted as the
        /// server frees system memory than those assigned a System.Web.Caching.CacheItemPriority.Normal
        /// priority.
        /// </summary>
        AboveNormal = 4,

        /// <summary>
        /// Cache items with this priority level are the least likely to be deleted from
        /// the cache as the server frees system memory.
        /// </summary>
        High = 5,

        /// <summary>
        /// The cache items with this priority level will not be automatically deleted
        /// from the cache as the server frees system memory. However, items with this
        /// priority level are removed along with other items according to the item's
        /// absolute or sliding expiration time.
        /// </summary>
        NotRemovable = 6,
    }

    #endregion

    #region Interface

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

        /// <summary>
        /// Removes cached items where the base type is (T)
        /// </summary>
        /// <typeparam name="T">Type of the object being cached</typeparam>
        /// <returns>Implemented Interface for configuring the cache</returns>
        void RemoveAll<T>() where T : class;
    }

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
        ICacheObject<T> WithDurationOf(TimeSpan cacheDuration, CacheExpiration cacheExpiration = CacheExpiration.Absolute, CachePriority cachePriority = CachePriority.Normal);

        /// <summary>
        /// Retrieves the cached object. If found from cache (by the key) then the cached object is returned.
        /// Otherwise, the cachedObjectLoader is called to load the object, and it is then added to cache.
        /// </summary>
        T CachedObject { get; }
    }

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
        T Remove();

        /// <summary>
        /// Retrieves the cached object. If found from cache (by the key) then the cached object is returned.
        /// Otherwise, the cachedObjectLoader is called to load the object, and it is then added to cache.
        /// </summary>
        T CachedObject { get; }
    }

    #endregion

    /// <summary>
    /// Implemented Cache Mangaer
    /// </summary>
    public class CacheManager : ICacheManager
    {
        #region Members

        private ICacheProvider _provider;

        #endregion

        #region Implementations

        class CacheKey
        {
            #region Members

            private List<string> _keys = new List<string>();

            public CacheKey(Type type)
            {
                Type baseType = GetBaseType(type);

                if (type == baseType)
                {
                    //  namespace.BaseType
                    Add(type.FullName);
                }
                else
                {
                    //  namespace.BaseType;Enumerable
                    Add(baseType.FullName);
                    Add("Enumerable");
                }
            }

            private Type GetBaseType(Type type)
            {
                if (!type.IsValueType && type.IsGenericType && IsImplementationOf(type, typeof(IEnumerable)))
                {
                    Type tg = type.GetGenericArguments().First(x => !x.IsValueType);

                    return tg;
                }

                return type;
            }

            private static bool IsImplementationOf(Type baseType, Type interfaceType)
            {
                return baseType.GetInterfaces().Any(interfaceType.Equals);
            }

            public void Add(params object[] cacheKeys)
            {
                if (cacheKeys != null)
                {
                    if (cacheKeys.Length == 1)
                    {
                        _keys.Add(cacheKeys.First().ToString());
                    }
                    else
                    {
                        bool delim = false;

                        StringBuilder key = new StringBuilder();

                        foreach (object o in cacheKeys)
                        {
                            if (delim) key.Append(",");

                            key.Append(o.ToString());

                            delim = true;
                        }

                        _keys.Add(key.ToString());
                    }
                }
            }

            public string Key { get { return string.Join(";", _keys) + ";"; } }

            #endregion
        }

        class CacheImplementation<T> : ICacheObject<T>, ICacheRetrieval<T> where T : class
        {
            #region Members

            private CacheKey _key;

            private ICacheProvider _provider;

            private Func<T> _loader;


            private TimeSpan? _expirationDuration = TimeSpan.FromMinutes(20);

            private CacheExpiration _expires = CacheExpiration.Absolute;

            private CachePriority _priority = CachePriority.Normal;

            #endregion

            #region Methods

            public CacheImplementation(ICacheProvider provider)
                : this(provider, null)
            {
            }

            public CacheImplementation(ICacheProvider provider, Func<T> objectLoader)
            {
                _loader = objectLoader;

                _key = new CacheKey(typeof(T));

                _provider = provider;
            }

            private T GetObject()
            {
                string key = _key.Key;

                // try to get the query result from the cache
                T result = (T)_provider.Get(key);

                if (result == null && _loader != null)
                {
                    result = _loader();

                    if (result != null)
                    {
                        _provider.Add(key, result, _expirationDuration.Value, _expires, _priority);
                    }
                }

                return result;
            }

            #endregion

            #region ICacheObject<T> Members

            public ICacheObject<T> By(params object[] cacheKeys)
            {
                _key.Add(cacheKeys);

                return this;
            }

            public ICacheObject<T> WithDurationOf(TimeSpan cacheDuration, CacheExpiration cacheExpiration = CacheExpiration.Absolute, CachePriority cachePriority = CachePriority.Normal)
            {
                _expirationDuration = cacheDuration;

                _expires = cacheExpiration;

                _priority = cachePriority;

                return this;
            }

            public T CachedObject { get { return GetObject(); } }

            #endregion

            #region ICacheRetrieval<T> Members

            ICacheRetrieval<T> ICacheRetrieval<T>.By(params object[] cacheKeys)
            {
                _key.Add(cacheKeys);

                return this;
            }

            T ICacheRetrieval<T>.Remove()
            {
                string key = _key.Key;

                return (T)_provider.Remove(key);
            }

            T ICacheRetrieval<T>.CachedObject
            {
                get { return GetObject(); }
            }

            #endregion
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public CacheManager(ICacheProvider provider)
        {
            _provider = provider;
        }

        #endregion

        #region ICacheManager Members

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cachedObjectLoader"></param>
        /// <returns></returns>
        public ICacheObject<T> Cache<T>(Func<T> cachedObjectLoader) where T : class
        {
            return new CacheImplementation<T>(_provider, cachedObjectLoader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICacheRetrieval<T> Find<T>() where T : class
        {
            return new CacheImplementation<T>(_provider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveAll<T>() where T : class
        {
            string key = new CacheKey(typeof(T)).Key;

            foreach (string k in _provider.GetAllKeys())
            {
                if (k.StartsWith(key))
                {
                    _provider.Remove(k);
                }
            }
        }

        #endregion
    }
}
