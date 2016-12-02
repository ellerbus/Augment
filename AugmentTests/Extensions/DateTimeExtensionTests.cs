using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class DateTimeExtensionTests
    {
        [TestMethod]
        public void DateTimeExtensions_EnsureUtcUnspecified_Test()
        {
            DateTime unspecified = new DateTime(2010, 1, 1, 15, 18, 19, DateTimeKind.Unspecified);
            DateTime utc = new DateTime(2010, 1, 1, 15, 18, 19, DateTimeKind.Utc);

            Assert.AreEqual(unspecified.EnsureUtc(), utc);
            Assert.AreEqual(unspecified.EnsureUtc().Kind, utc.Kind);
        }

        [TestMethod]
        public void DateTimeExtensions_EnsureUtcLocal_Test()
        {
            DateTime local = new DateTime(2010, 1, 1, 15, 18, 19, DateTimeKind.Local);

            local = local.Add(TimeZone.CurrentTimeZone.GetUtcOffset(local));

            DateTime utc = new DateTime(2010, 1, 1, 15, 18, 19, DateTimeKind.Utc);

            Assert.AreEqual(local.EnsureUtc(), utc);
            Assert.AreEqual(local.EnsureUtc().Kind, utc.Kind);
        }

        [TestMethod]
        public void DateTimeExtensions_StartEnd_Test()
        {
            DateTime now = new DateTime(2012, 6, 15, 13, 23, 58);

            Assert.AreEqual(new DateTime(2012, 6, 15), now.BeginningOfDay());
            Assert.AreEqual(new DateTime(2012, 6, 10), now.BeginningOfWeek(DayOfWeek.Sunday));
            Assert.AreEqual(new DateTime(2012, 6, 1), now.BeginningOfMonth());
            Assert.AreEqual(new DateTime(2012, 4, 1), now.BeginningOfQuarter());
            Assert.AreEqual(new DateTime(2012, 1, 1), now.BeginningOfYear());

            Assert.AreEqual(new DateTime(2012, 6, 15, 23, 59, 59), now.EndOfDay());
            Assert.AreEqual(new DateTime(2012, 6, 16, 23, 59, 59), now.EndOfWeek(DayOfWeek.Sunday));
            Assert.AreEqual(new DateTime(2012, 6, 30, 23, 59, 59), now.EndOfMonth());
            Assert.AreEqual(new DateTime(2012, 6, 30, 23, 59, 59), now.EndOfQuarter());
            Assert.AreEqual(new DateTime(2012, 12, 31, 23, 59, 59), now.EndOfYear());
        }

        [TestMethod]
        public void DateTimeExtensions_Holiday_Test()
        {
            int year = 2013;

            Assert.IsTrue(new DateTime(year, 1, 1).IsHoliday());    //  NEW YEARS
            Assert.IsTrue(new DateTime(year, 5, 27).IsHoliday());   //  MEMORIAL DAY
            Assert.IsTrue(new DateTime(year, 7, 4).IsHoliday());    //  INDEPENCENCE DAY
            Assert.IsTrue(new DateTime(year, 9, 2).IsHoliday());    //  LABOR DAY
            Assert.IsTrue(new DateTime(year, 11, 28).IsHoliday());  //  THANKSGIVING DAY
            Assert.IsTrue(new DateTime(year, 12, 25).IsHoliday());  //  CHRISTMAS DAY

            Assert.IsFalse(new DateTime(year, 1, 2).IsHoliday());
            Assert.IsFalse(new DateTime(year, 5, 28).IsHoliday());
            Assert.IsFalse(new DateTime(year, 7, 5).IsHoliday());
            Assert.IsFalse(new DateTime(year, 9, 1).IsHoliday());
            Assert.IsFalse(new DateTime(year, 11, 23).IsHoliday());
            Assert.IsFalse(new DateTime(year, 12, 26).IsHoliday());
        }

        [TestMethod]
        public void DateTimeExtensions_Weekend_Test()
        {
            int year = 2013;
            int mnth = 9;
            int day = 1;

            DateTime sun = new DateTime(year, mnth, day++);
            DateTime mon = new DateTime(year, mnth, day++);
            DateTime tue = new DateTime(year, mnth, day++);
            DateTime wed = new DateTime(year, mnth, day++);
            DateTime thu = new DateTime(year, mnth, day++);
            DateTime fri = new DateTime(year, mnth, day++);
            DateTime sat = new DateTime(year, mnth, day);

            Assert.IsTrue(sun.IsWeekend());
            Assert.IsFalse(mon.IsWeekend());
            Assert.IsFalse(tue.IsWeekend());
            Assert.IsFalse(wed.IsWeekend());
            Assert.IsFalse(thu.IsWeekend());
            Assert.IsFalse(fri.IsWeekend());
            Assert.IsTrue(sat.IsWeekend());
        }

        [TestMethod]
        public void DateTimeExtensions_BusinessDay_Test()
        {
            int year = 2013;
            int mnth = 12;
            int day = 22;

            DateTime sun = new DateTime(year, mnth, day++);
            DateTime mon = new DateTime(year, mnth, day++);
            DateTime tue = new DateTime(year, mnth, day++);
            DateTime wed = new DateTime(year, mnth, day++);
            DateTime thu = new DateTime(year, mnth, day++);
            DateTime fri = new DateTime(year, mnth, day++);
            DateTime sat = new DateTime(year, mnth, day);

            Assert.IsFalse(sun.IsBusinessDay());
            Assert.IsTrue(mon.IsBusinessDay());
            Assert.IsTrue(tue.IsBusinessDay());
            Assert.IsFalse(wed.IsBusinessDay());
            Assert.IsTrue(thu.IsBusinessDay());
            Assert.IsTrue(fri.IsBusinessDay());
            Assert.IsFalse(sat.IsBusinessDay());
        }

        [TestMethod]
        public void DateTimeExtensions_RelativeFuture_Test()
        {
            Assert.AreEqual(
                "on " + DateTime.Now.AddDays(366).ToString("MMMM d, yyyy"),
                DateTime.Now.AddDays(366).ToRelativeDateString()
                );
            Assert.AreEqual(
                "on " + DateTime.Now.AddDays(10).ToString("MMMM d"),
                DateTime.Now.AddDays(10).ToRelativeDateString()
                );
            Assert.AreEqual(
                "2 days from now",
                DateTime.Now.AddDays(2).ToRelativeDateString()
                );
            Assert.AreEqual(
                "tomorrow",
                DateTime.Now.AddDays(1).ToRelativeDateString()
                );
            Assert.AreEqual(
                "3 hours from now",
                DateTime.Now.AddHours(3).ToRelativeDateString()
                );
            Assert.AreEqual(
                "more than an hour from now",
                DateTime.Now.AddMinutes(90).ToRelativeDateString()
                );
            Assert.AreEqual(
                "6 minutes from now",
                DateTime.Now.AddMinutes(6).ToRelativeDateString()
                );
            Assert.AreEqual(
                "a few minutes from now",
                DateTime.Now.AddMinutes(2).ToRelativeDateString()
                );
            Assert.AreEqual(
                "less than a minute from now",
                DateTime.Now.AddSeconds(2).ToRelativeDateString()
                );
        }

        [TestMethod]
        public void DateTimeExtensions_RelativePast_Test()
        {
            Assert.AreEqual(
                "on " + DateTime.Now.AddDays(-366).ToString("MMMM d, yyyy"),
                DateTime.Now.AddDays(-366).ToRelativeDateString()
                );
            Assert.AreEqual(
                "on " + DateTime.Now.AddDays(-10).ToString("MMMM d"),
                DateTime.Now.AddDays(-10).ToRelativeDateString()
                );
            Assert.AreEqual(
                "2 days ago",
                DateTime.Now.AddDays(-2).ToRelativeDateString()
                );
            Assert.AreEqual(
                "yesterday",
                DateTime.Now.AddDays(-1).ToRelativeDateString()
                );
            Assert.AreEqual(
                "3 hours ago",
                DateTime.Now.AddHours(-3).ToRelativeDateString()
                );
            Assert.AreEqual(
                "more than an hour ago",
                DateTime.Now.AddMinutes(-90).ToRelativeDateString()
                );
            Assert.AreEqual(
                "6 minutes ago",
                DateTime.Now.AddMinutes(-6).ToRelativeDateString()
                );
            Assert.AreEqual(
                "a few minutes ago",
                DateTime.Now.AddMinutes(-2).ToRelativeDateString()
                );
            Assert.AreEqual(
                "less than a minute ago",
                DateTime.Now.AddSeconds(-2).ToRelativeDateString()
                );
        }
    }
}