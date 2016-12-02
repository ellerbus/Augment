using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class IntExtensionTests
    {
        [TestMethod]
        public void IntExtensions_ToTimeSpan_Test()
        {
            Assert.AreEqual(TimeSpan.FromMilliseconds(2), 2.Milliseconds());
            Assert.AreEqual(TimeSpan.FromSeconds(4), 4.Seconds());
            Assert.AreEqual(TimeSpan.FromMinutes(6), 6.Minutes());
            Assert.AreEqual(TimeSpan.FromHours(8), 8.Hours());
            Assert.AreEqual(TimeSpan.FromDays(10), 10.Days());
            Assert.AreEqual(TimeSpan.FromDays(12 * 30), 12.Months());
            Assert.AreEqual(TimeSpan.FromDays(14 * 365.25), 14.Years());
        }

        [TestMethod]
        public void IntExtensions_PercentOf_Test()
        {
            Assert.AreEqual(20, 20.PercentOf(100));
            Assert.AreEqual(20, 20.PercentOf(100.0));
            Assert.AreEqual(40, 20.PercentOf(200));
            Assert.AreEqual(40, 20.PercentOf(200.0));
        }
    }
}