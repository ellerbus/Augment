using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Augment
{
    /// <summary>
    /// Extensions for double
    /// </summary>
    public static class DoubleExtensions
    {
        #region Percent Of

        /// <summary>
        /// Gets 'x' percent of 'number' (ie. 20.PercentOf(200) == 40)
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static double PercentOf(this double percent, double number)
        {
            return number * (percent / 100);
        }

        #endregion

        #region Misc. Helpers

        /// <summary>
        /// 12.223232.RoundTo(2)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double RoundTo(this double value, int decimals, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, decimals, mode);
        }

        #endregion
    }
}
