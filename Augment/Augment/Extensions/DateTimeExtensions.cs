using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Augment
{
    /// <summary>
    /// DateTime Extensions
    /// </summary>
    /// <remarks>
    /// http://pawjershauge.blogspot.com/2010/03/datetime-extensions.html
    /// </remarks>
    public static class DateTimeExtensions
    {
        #region Day

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime BeginningOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
        }

        #endregion

        #region Week

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime BeginningOfWeek(this DateTime dt, DayOfWeek? startOfWeek = null)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;

            int diff = dt.DayOfWeek - (startOfWeek == null ? dfi.FirstDayOfWeek : startOfWeek.Value);

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek? startOfWeek = null)
        {
            return BeginningOfWeek(dt, startOfWeek).AddDays(6).EndOfDay();
        }

        #endregion

        #region Month

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime BeginningOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime EndOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month), 23, 59, 59);
        }

        #endregion

        #region Quarter

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime BeginningOfQuarter(this DateTime dt)
        {
            int m = GetQuarter(dt) * 3 - 2;

            return new DateTime(dt.Year, m, 1, 0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime EndOfQuarter(this DateTime dt)
        {
            int m = GetQuarter(dt) * 3;

            return new DateTime(dt.Year, m, DateTime.DaysInMonth(dt.Year, m), 23, 59, 59);
        }

        #endregion

        #region Year

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime BeginningOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1, 0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime EndOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 12, 31, 23, 59, 59);
        }

        #endregion

        #region Misc

        /// <summary>
        /// Returns true if the day is Saturday or Sunday
        /// </summary>
        /// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        /// <returns>boolean value indicating if the date is a weekend</returns>
        public static bool IsWeekend(this DateTime dt)
        {
            return (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday);
        }

        /// <summary>
        /// Returns true if the day is Saturday or Sunday
        /// </summary>
        /// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        /// <returns>boolean value indicating if the date is a holiday</returns>
        public static bool IsHoliday(this DateTime dt)
        {
            return GetHolidays(dt.Year).Contains(BeginningOfDay(dt));
        }

        /// <summary>
        /// Returns true if the day is Saturday or Sunday
        /// </summary>
        /// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        /// <returns>boolean value indicating if the date is a business day (not weekend and not holiday)</returns>
        public static bool IsBusinessDay(this DateTime dt)
        {
            return !dt.IsWeekend() && !dt.IsHoliday();
        }

        /// <summary>
        /// Get the quarter that the datetime is in.
        /// </summary>
        /// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        /// <returns>Returns 1 to 4 that represenst the quarter that the datetime is in.</returns>
        public static int GetQuarter(this DateTime dt)
        {
            return ((dt.Month - 1) / 3) + 1;
        }

        #endregion

        #region Holiday Calculations

        private static LeastRecentlyUsedCache<int, HashSet<DateTime>> _holidayCache = new LeastRecentlyUsedCache<int, HashSet<DateTime>>(50);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static HashSet<DateTime> GetHolidays(int year)
        {
            lock (_holidayCache)
            {
                if (_holidayCache.ContainsKey(year))
                {
                    return _holidayCache[year];
                }

                HashSet<DateTime> holidays = new HashSet<DateTime>();

                //  NEW YEARS 
                DateTime newYearsDate = AdjustForWeekendHoliday(new DateTime(year, 1, 1));

                holidays.Add(newYearsDate);

                //  MEMORIAL DAY  -- last monday in May 
                DateTime memorialDay = new DateTime(year, 5, 31);

                DayOfWeek dayOfWeek = memorialDay.DayOfWeek;

                while (dayOfWeek != DayOfWeek.Monday)
                {
                    memorialDay = memorialDay.AddDays(-1);

                    dayOfWeek = memorialDay.DayOfWeek;
                }

                holidays.Add(memorialDay);

                //  INDEPENCENCE DAY 
                DateTime independenceDay = AdjustForWeekendHoliday(new DateTime(year, 7, 4));

                holidays.Add(independenceDay);

                //  LABOR DAY -- 1st Monday in September 
                DateTime laborDay = new DateTime(year, 9, 1);

                dayOfWeek = laborDay.DayOfWeek;

                while (dayOfWeek != DayOfWeek.Monday)
                {
                    laborDay = laborDay.AddDays(1);

                    dayOfWeek = laborDay.DayOfWeek;
                }

                holidays.Add(laborDay.Date);

                //  THANKSGIVING DAY - 4th Thursday in November 
                int thanksgiving = Enumerable.Range(1, 30)
                    .Where(i => new DateTime(year, 11, i).DayOfWeek == DayOfWeek.Thursday)
                    .ElementAt(3)
                    ;

                DateTime thanksgivingDay = new DateTime(year, 11, thanksgiving);

                holidays.Add(thanksgivingDay);

                //  CHRISTMAS DAY
                DateTime christmasDay = AdjustForWeekendHoliday(new DateTime(year, 12, 25));

                holidays.Add(christmasDay);

                _holidayCache[year] = holidays;

                return holidays;
            }
        }

        private static DateTime AdjustForWeekendHoliday(DateTime holiday)
        {
            if (holiday.DayOfWeek == DayOfWeek.Saturday)
            {
                return holiday.AddDays(-1);
            }

            if (holiday.DayOfWeek == DayOfWeek.Sunday)
            {
                return holiday.AddDays(1);
            }

            return holiday;
        }

        #endregion

        #region Relative Date String

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToRelativeDateString(this DateTime dt)
        {
            return GetRelativeDateValue(dt, DateTime.Now);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToRelativeDateStringUtc(this DateTime dt)
        {
            return GetRelativeDateValue(dt, DateTime.UtcNow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="comparedTo"></param>
        /// <returns></returns>
        private static string GetRelativeDateValue(DateTime dt, DateTime comparedTo)
        {
            TimeSpan diff = comparedTo.Subtract(dt);

            bool isFutureRelative = dt > comparedTo;

            if (Math.Abs(diff.TotalDays) >= 365)
            {
                return "on " + dt.ToString("MMMM d, yyyy");
            }
            if (Math.Abs(diff.TotalDays) >= 7)
            {
                return "on " + dt.ToString("MMMM d");
            }

            string relative = isFutureRelative ? "from now" : "ago";

            if (Math.Abs(diff.TotalDays) > 1)
            {
                return "{0:N0} days {1}".FormatArgs(Math.Abs(diff.TotalDays), relative);
            }
            if (Math.Abs(diff.TotalDays) == 1)
            {
                if (isFutureRelative)
                {
                    return "tomorrow";
                }

                return "yesterday";
            }
            if (Math.Abs(diff.TotalHours) >= 2)
            {
                return "{0:N0} hours {1}".FormatArgs(Math.Abs(diff.TotalHours), relative);
            }
            if (Math.Abs(diff.TotalMinutes) >= 60)
            {
                return "more than an hour {0}".FormatArgs(relative);
            }
            if (Math.Abs(diff.TotalMinutes) >= 5)
            {
                return "{0:N0} minutes {1}".FormatArgs(Math.Abs(diff.TotalMinutes), relative);
            }
            if (Math.Abs(diff.TotalMinutes) >= 1)
            {
                return "a few minutes {0}".FormatArgs(relative);
            }

            return "less than a minute {0}".FormatArgs(relative);
        }

        #endregion














        ///// <summary>
        ///// Returns the first day of week with in the month.
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <param name="dow">What day of week to find the first one of in the month.</param>
        ///// <returns>Returns DateTime object that represents the first day of week with in the month.</returns>
        //public static DateTime FirstDayOfWeekInMonth(this DateTime dt, DayOfWeek dow)
        //{
        //    DateTime firstDay = new DateTime(dt.Year, dt.Month, 1);
        //    int diff = firstDay.DayOfWeek - dow;
        //    if (diff > 0) diff -= 7;
        //    return firstDay.AddDays(diff * -1);
        //}

        ///// <summary>
        ///// Returns the first weekday (Financial day) of the month
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <returns>Returns DateTime object that represents the first weekday (Financial day) of the month</returns>
        //public static DateTime FirstWeekDayOfMonth(this DateTime dt)
        //{
        //    DateTime firstDay = new DateTime(dt.Year, dt.Month, 1);
        //    for (int i = 0; i < 7; i++)
        //    {
        //        if (firstDay.AddDays(i).DayOfWeek != DayOfWeek.Saturday && firstDay.AddDays(i).DayOfWeek != DayOfWeek.Sunday)
        //            return firstDay.AddDays(i);
        //    }
        //    return firstDay;
        //}

        ///// <summary>
        ///// Returns the last day of week with in the month.
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <param name="dow">What day of week to find the last one of in the month.</param>
        ///// <returns>Returns DateTime object that represents the last day of week with in the month.</returns>
        //public static DateTime LastDayOfWeekInMonth(this DateTime dt, DayOfWeek dow)
        //{
        //    DateTime lastDay = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        //    DayOfWeek lastDow = lastDay.DayOfWeek;

        //    int diff = dow - lastDow;
        //    if (diff > 0) diff -= 7;

        //    return lastDay.AddDays(diff);
        //}

        ///// <summary>
        ///// Returns the last weekday (Financial day) of the month
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <returns>Returns DateTime object that represents the last weekday (Financial day) of the month</returns>
        //public static DateTime LastWeekDayOfMonth(this DateTime dt)
        //{
        //    DateTime lastDay = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        //    for (int i = 0; i < 7; i++)
        //    {
        //        if (lastDay.AddDays(i * -1).DayOfWeek != DayOfWeek.Saturday && lastDay.AddDays(i * -1).DayOfWeek != DayOfWeek.Sunday)
        //            return lastDay.AddDays(i * -1);
        //    }
        //    return lastDay;
        //}

        ///// <summary>
        ///// Returns the closest Weekday (Financial day) Date
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <returns>Returns the closest Weekday (Financial day) Date</returns>
        //public static DateTime FindClosestWeekDay(this DateTime dt)
        //{
        //    if (dt.DayOfWeek == DayOfWeek.Saturday)
        //        return dt.AddDays(-1);
        //    if (dt.DayOfWeek == DayOfWeek.Sunday)
        //        return dt.AddDays(1);
        //    else
        //        return dt;
        //}

        ///// <summary>
        ///// Returns a given datetime according to the week of year and the specified day within the week.
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <param name="week">A number of whole and fractional weeks. The value parameter can only be positive.</param>
        ///// <param name="dayofweek">A DayOfWeek to find in the week</param>
        ///// <returns>A DateTime whose value is the sum according to the week of year and the specified day within the week.</returns>
        //public static DateTime GetDateByWeek(this DateTime dt, int week, DayOfWeek dayofweek)
        //{
        //    if (week > 0 && week < 54)
        //    {
        //        DateTime FirstDayOfyear = new DateTime(dt.Year, 1, 1);
        //        int daysToFirstCorrectDay = (((int)dayofweek - (int)FirstDayOfyear.DayOfWeek) + 7) % 7;
        //        return FirstDayOfyear.AddDays(7 * (week - 1) + daysToFirstCorrectDay);
        //    }
        //    else
        //        return dt;
        //}

        //private static int Sub(DayOfWeek s, DayOfWeek e)
        //{
        //    if ((s - e) > 0) return (s - e) - 7;
        //    if ((s - e) == 0) return -7;
        //    return (s - e);
        //}

        ///// <summary>
        ///// Returns first next occurence of specified DayOfTheWeek
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <param name="day">A DayOfWeek to find the next occurence of</param>
        ///// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the enum value represented by the day.</returns>
        //public static DateTime Next(this DateTime dt, DayOfWeek day)
        //{
        //    return dt.AddDays(Sub(dt.DayOfWeek, day) * -1);
        //}

        ///// <summary>
        ///// Returns next "first" occurence of specified DayOfTheWeek
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <param name="day">A DayOfWeek to find the previous occurence of</param>
        ///// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the enum value represented by the day.</returns>
        //public static DateTime Previous(this DateTime dt, DayOfWeek day)
        //{
        //    return dt.AddDays(Sub(day, dt.DayOfWeek));
        //}


        ///// <summary>
        ///// Adds the specified number of financials days to the value of this instance.
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <param name="days">A number of whole and fractional financial days. The value parameter can be negative or positive.</param>
        ///// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the number of financial days represented by days.</returns>
        //public static DateTime AddFinancialDays(this DateTime dt, int days)
        //{
        //    int addint = Math.Sign(days);
        //    for (int i = 0; i < (Math.Sign(days) * days); i++)
        //    {
        //        do { dt = dt.AddDays(addint); }
        //        while (dt.IsWeekend());
        //    }
        //    return dt;
        //}

        ///// <summary>
        ///// Calculate Financial days between two dates.
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <param name="otherdate">End or start date to calculate to or from.</param>
        ///// <returns>Amount of financial days between the two dates</returns>
        //public static int CountFinancialDays(this DateTime dt, DateTime otherdate)
        //{
        //    TimeSpan ts = (otherdate - dt);
        //    int addint = Math.Sign(ts.Days);
        //    int unsigneddays = (Math.Sign(ts.Days) * ts.Days);
        //    int businessdays = 0;
        //    for (int i = 0; i < unsigneddays; i++)
        //    {
        //        dt = dt.AddDays(addint);
        //        if (!dt.IsWeekend())
        //            businessdays++;
        //    }
        //    return businessdays;
        //}

        ///// <summary>
        ///// Converts any datetime to the amount of seconds from 1972.01.01 00:00:00
        ///// Microsoft sometimes uses the amount of seconds from 1972.01.01 00:00:00 to indicate an datetime.
        ///// </summary>
        ///// <param name="dt">DateTime Base, from where the calculation will be preformed.</param>
        ///// <returns>Total seconds past since 1972.01.01 00:00:00</returns>
        //public static double ToMicrosoftNumber(this DateTime dt)
        //{
        //    return (dt - new DateTime(1972, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        //}
    }

}
