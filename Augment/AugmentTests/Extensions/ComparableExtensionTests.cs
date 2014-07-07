using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class ComparableExtensionTests
    {
        [TestMethod]
        public void ComparableExtensions_IsBetween_Test()
        {
            int i = 5;

            Assert.IsTrue(i.IsBetween(4, 6));
            Assert.IsTrue(i.IsBetween(5, 6));
            Assert.IsTrue(i.IsBetween(4, 5));
            Assert.IsFalse(i.IsBetween(4, 5, false));
            Assert.IsFalse(i.IsBetween(3, 4));
        }
    }
}