using System;

namespace Augment
{
    /// <summary>
    /// TimeSpan Extensions
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Defines 'now' for TimeSpanExtensions ('now' can be today, utcnow, or leave as-is for now)
        /// </summary>
        public static Func<DateTime> Now = () => DateTime.Now;

        /// <summary>
        /// TimeSpan From 'now()' (5.Days().FromNow())
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime FromNow(this TimeSpan ts)
        {
            return Now() + ts;
        }

        /// <summary>
        /// TimeSpan Ago (5.Minutes().Ago())
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime Ago(this TimeSpan ts)
        {
            return Now() - ts;
        }
    }
}