using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class DoubleExtensionTests
    {
        [TestMethod]
        public void DoubleExtensions_PercentOf_Test()
        {
            Assert.AreEqual(20.0, 20.0.PercentOf(100));
            Assert.AreEqual(20.0, 20.0.PercentOf(100.0));
            Assert.AreEqual(20.5, 20.5.PercentOf(100));
            Assert.AreEqual(40.0, 20.0.PercentOf(200));
            Assert.AreEqual(40.0, 20.0.PercentOf(200.0));

        }

        [TestMethod]
        public void DoubleExtensions_RoundTo_Test()
        {
            Assert.AreEqual(5.0, 4.5.RoundTo(0, MidpointRounding.AwayFromZero));
            Assert.AreEqual(4.0, 4.5.RoundTo(0, MidpointRounding.ToEven));

            Assert.AreEqual(5.0, 4.6.RoundTo(0, MidpointRounding.AwayFromZero));
            Assert.AreEqual(5.0, 4.6.RoundTo(0, MidpointRounding.ToEven));

            Assert.AreEqual(6.0, 5.5.RoundTo(0, MidpointRounding.AwayFromZero));
            Assert.AreEqual(6.0, 5.5.RoundTo(0, MidpointRounding.ToEven));

            Assert.AreEqual(6.0, 5.6.RoundTo(0, MidpointRounding.AwayFromZero));
            Assert.AreEqual(6.0, 5.6.RoundTo(0, MidpointRounding.ToEven));
        }

        [TestMethod]
        public void DoubleExtensions_RoundOn_Test()
        {
            Assert.AreEqual(0.75, 0.80.RoundOn(0.25));
            Assert.AreEqual(0.80, 0.80.RoundOn(0.05));
            Assert.AreEqual(0.25, 0.255.RoundOn(0.25));
            Assert.AreEqual(5.0, 6.25.RoundOn(5));
        }
    }
}