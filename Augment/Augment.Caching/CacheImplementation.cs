using System;
using System.Text.RegularExpressions;

namespace Augment.Caching
{
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

        #region Constructors

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

        #endregion

        #region Methods

        private T GetObject()
        {
            string key = _key.CreateKey();

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

        public ICacheObject<T> DurationOf(TimeSpan cacheDuration, CacheExpiration cacheExpiration = CacheExpiration.Absolute, CachePriority cachePriority = CachePriority.Normal)
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
            string key = _key.CreateKey();

            return (T)_provider.Remove(key);
        }

        void ICacheRetrieval<T>.RemoveAll()
        {
            string pattern = _key.CreateRemoveAllKeyPattern();

            Regex regex = new Regex(pattern, RegexOptions.Compiled);

            foreach (string k in _provider.GetAllKeys())
            {
                if (regex.IsMatch(k))
                {
                    _provider.Remove(k);
                }
            }
        }

        T ICacheRetrieval<T>.CachedObject
        {
            get { return GetObject(); }
        }

        #endregion
    }
}
