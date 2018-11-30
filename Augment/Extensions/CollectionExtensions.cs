using System.Collections.Generic;
using System.Linq;

namespace Augment
{
    /// <summary>
    /// Handy collection extensions
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Determines if the collection is null or has a count of 0
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            ICollection<T> col = enumerable as ICollection<T>;

            if (col != null)
            {
                return col.IsNullOrEmpty();
            }

            return !enumerable.Any();
        }

        /// <summary>
        /// Determines if the collection a count greater than 0
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.IsNullOrEmpty();
        }

        /// <summary>
        /// Determines if the collection is null or has a count of 0
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> col)
        {
            if (col == null)
            {
                return true;
            }

            return col.Count == 0;
        }

        /// <summary>
        /// Determines if the collection a count greater than 0
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this ICollection<T> col)
        {
            return !col.IsNullOrEmpty();
        }
    }
}