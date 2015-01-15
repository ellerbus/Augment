using System;

namespace Augment.Caching
{
    /// <summary>
    /// Implemented Cache Mangaer
    /// </summary>
    /// <remarks>
    /// Based on: http://www.sharepointblues.com/2010/09/03/custom-caching-done-fluently/
    /// </remarks>
    public class CacheManager : ICacheManager
    {
        #region Members

        private ICacheProvider _provider;

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

        #endregion
    }
}
