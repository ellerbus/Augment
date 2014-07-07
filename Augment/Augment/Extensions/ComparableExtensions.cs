using System;

namespace Augment
{
    /// <summary>
    /// Handy comparable extensions
    /// </summary>
    public static class ComparableExtensions
    {
        /// <summary>
        /// Test is between low and high (inclusive on both)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="inclusive">true by default</param>
        /// <returns></returns>
        public static bool IsBetween<T>(this T value, T low, T high, bool inclusive = true) where T : IComparable<T>
        {
            if (inclusive)
            {
                return value.CompareTo(low) >= 0 && value.CompareTo(high) <= 0;
            }

            return value.CompareTo(low) > 0 && value.CompareTo(high) < 0;
        }
    }
}
