
namespace Augment.Caching
{
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
}