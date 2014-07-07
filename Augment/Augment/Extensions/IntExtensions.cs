using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Augment
{
    /// <summary>
    /// Extensions for int
    /// </summary>
    public static class IntExtensions
    {
        #region Percent Of

        /// <summary>
        /// Gets 'x' percent of 'number' (ie. 20.PercentOf(100) == 20)
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static double PercentOf(this int percent, double number)
        {
            double d = DoubleExtensions.PercentOf(percent, number);

            return d;
        }

        /// <summary>
        /// Gets 'x' percent of 'number' (ie. 20.PercentOf(100) == 20)
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int PercentOf(this int percent, int number)
        {
            int i = (int)DoubleExtensions.PercentOf(percent, number);

            return i;
        }

        #endregion

        #region To TimeSpan

        /// <summary>
        /// Gets a timespan for 'x' milliseconds
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static TimeSpan Milliseconds(this int x)
        {
            return TimeSpan.FromMilliseconds(x);
        }

        /// <summary>
        /// Gets a timespan for 'x' seconds
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static TimeSpan Seconds(this int x)
        {
            return TimeSpan.FromSeconds(x);
        }

        /// <summary>
        /// Gets a timespan for 'x' minutes
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static TimeSpan Minutes(this int x)
        {
            return TimeSpan.FromMinutes(x);
        }

        /// <summary>
        /// Gets a timespan for 'x' hours
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static TimeSpan Hours(this int x)
        {
            return TimeSpan.FromHours(x);
        }

        /// <summary>
        /// Gets a timespan for 'x' days
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static TimeSpan Days(this int x)
        {
            return TimeSpan.FromDays(x);
        }

        /// <summary>
        /// Gets a timespan for 'x' months
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static TimeSpan Months(this int x)
        {
            return TimeSpan.FromDays(x * 30);
        }

        /// <summary>
        /// Gets a timespan for 'x' years
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static TimeSpan Years(this int x)
        {
            return TimeSpan.FromDays(x * 365.25);
        }

        #endregion
    }
}
