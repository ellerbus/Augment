using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class TimeSpanExtensionTests
    {
        private static readonly DateTime _now = new DateTime(2013, 1, 1);

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            TimeSpanExtensions.Now = () => _now;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TimeSpanExtensions.Now = () => DateTime.Now;
        }

        [TestMethod]
        public void TimeSpanExtensions_FromAndAgo_Test()
        {
            Assert.AreEqual(1, (TimeSpan.FromMilliseconds(1).FromNow() - _now).Milliseconds);
            Assert.AreEqual(2, (TimeSpan.FromSeconds(2).FromNow() - _now).Seconds);
            Assert.AreEqual(3, (TimeSpan.FromMinutes(3).FromNow() - _now).Minutes);
            Assert.AreEqual(4, (TimeSpan.FromHours(4).FromNow() - _now).Hours);
            Assert.AreEqual(5, (TimeSpan.FromDays(5).FromNow() - _now).Days);
            Assert.AreEqual(6 * 30, (TimeSpan.FromDays(6 * 30).FromNow() - _now).Days);
            Assert.AreEqual((int)(7 * 365.25), (TimeSpan.FromDays(7 * 365.25).FromNow() - _now).Days);

            Assert.AreEqual(-1, (TimeSpan.FromMilliseconds(1).Ago() - _now).Milliseconds);
            Assert.AreEqual(-2, (TimeSpan.FromSeconds(2).Ago() - _now).Seconds);
            Assert.AreEqual(-3, (TimeSpan.FromMinutes(3).Ago() - _now).Minutes);
            Assert.AreEqual(-4, (TimeSpan.FromHours(4).Ago() - _now).Hours);
            Assert.AreEqual(-5, (TimeSpan.FromDays(5).Ago() - _now).Days);
            Assert.AreEqual(-6 * 30, (TimeSpan.FromDays(6 * 30).Ago() - _now).Days);
            Assert.AreEqual((int)(-7 * 365.25), (TimeSpan.FromDays(7 * 365.25).Ago() - _now).Days);
        }

        [TestMethod]
        public void TimeSpanExtensions_FromAndAgoUsage_Test()
        {
            Assert.AreEqual(1, (1.Milliseconds().FromNow() - _now).Milliseconds);
            Assert.AreEqual(2, (2.Seconds().FromNow() - _now).Seconds);
            Assert.AreEqual(3, (3.Minutes().FromNow() - _now).Minutes);
            Assert.AreEqual(4, (4.Hours().FromNow() - _now).Hours);
            Assert.AreEqual(5, (5.Days().FromNow() - _now).Days);
            Assert.AreEqual(6 * 30, (6.Months().FromNow() - _now).Days);
            Assert.AreEqual((int)(7 * 365.25), (7.Years().FromNow() - _now).Days);

            Assert.AreEqual(-1, (1.Milliseconds().Ago() - _now).Milliseconds);
            Assert.AreEqual(-2, (2.Seconds().Ago() - _now).Seconds);
            Assert.AreEqual(-3, (3.Minutes().Ago() - _now).Minutes);
            Assert.AreEqual(-4, (4.Hours().Ago() - _now).Hours);
            Assert.AreEqual(-5, (5.Days().Ago() - _now).Days);
            Assert.AreEqual(-6 * 30, (6.Months().Ago() - _now).Days);
            Assert.AreEqual((int)(-7 * 365.25), (7.Years().Ago() - _now).Days);
        }
    }
}