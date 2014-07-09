using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public sealed class RangeTests
    {
        [TestMethod]
        public void Range_Should_Equal()
        {
            var rangeA = new Range<int>(1, 1);
            var rangeB = new Range<int>(2, 2);
            var rangeC = new Range<int>(1, 1);

            Assert.AreNotEqual(rangeA, rangeB);
            Assert.AreEqual(rangeA, rangeC);
            Assert.AreNotEqual(rangeB, rangeC);

#pragma warning disable 1718
            Assert.IsTrue(rangeA == rangeA);
#pragma warning restore 1718

            Assert.IsFalse(rangeA == rangeB);
            Assert.IsTrue(rangeA == rangeC);
            Assert.IsFalse(rangeB == rangeC);
            Assert.IsFalse((null as Range<int>) == rangeA);
            Assert.IsFalse(rangeA == (null as Range<int>));

            Assert.IsTrue(rangeA != rangeB);
            Assert.IsFalse(rangeA != rangeC);
            Assert.IsTrue(rangeB != rangeC);
        }

        [TestMethod]
        public void Range_Should_Provide_HashCode()
        {
            var rangeA = new Range<int>(1, 1);
            var rangeB = new Range<int>(1, 2);
            var rangeC = new Range<int>(1, 1);

            Assert.IsFalse(rangeA.GetHashCode() == rangeB.GetHashCode());
            Assert.IsTrue(rangeA.GetHashCode() == rangeC.GetHashCode());
        }

        [TestMethod]
        public void Range_Should_Contain()
        {
            var range = new Range<int>(3, 10);
            Assert.IsTrue(range.Contains(5));
            Assert.IsTrue(range.Contains(3));
            Assert.IsTrue(range.Contains(10));
            Assert.IsFalse(range.Contains(-3));
            Assert.IsFalse(range.Contains(20));
        }

        [TestMethod]
        public void Range_Should_Construct_Properly()
        {
            var a = new Range<int>(-3, 4);
            Assert.AreEqual(-3, a.Start);
            Assert.AreEqual(4, a.End);

            var b = new Range<int>(3, -4);
            Assert.AreEqual(-4, b.Start);
            Assert.AreEqual(3, b.End);

            var c = new Range<int>(3, 3);
            Assert.AreEqual(3, c.Start);
            Assert.AreEqual(3, c.End);
        }

        [TestMethod]
        public void Range_Should_Intersect()
        {
            var a = new Range<int>(3, 6);
            Range<int> ai = a.Intersect(new Range<int>(5, 8));

            Assert.AreEqual(5, ai.Start);
            Assert.AreEqual(6, ai.End);

            var b = new Range<int>(5, 8);
            var bi = b.Intersect(new Range<int>(3, 6));

            Assert.AreEqual(5, bi.Start);
            Assert.AreEqual(6, bi.End);

            var c = new Range<int>(3, 6);
            var ci = c.Intersect(new Range<int>(6, 8));

            Assert.AreEqual(6, ci.Start);
            Assert.AreEqual(6, ci.End);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Range_Should_Not_Intersect_Null()
        {
            new Range<int>(1, 3).Intersect(null);
        }

        [TestMethod]
        public void Range_Should_Not_Intersect()
        {
            var range = new Range<int>(3, 6);
            Assert.IsNull(range.Intersect(new Range<int>(7, 8)));
        }

        [TestMethod]
        public void Range_Should_Union()
        {
            var a = new Range<int>(3, 6);
            var au = a.Union(new Range<int>(4, 10));

            Assert.AreEqual(3, au.Start);
            Assert.AreEqual(10, au.End);

            var b = new Range<int>(4, 10);
            var bu = b.Union(new Range<int>(3, 6));

            Assert.AreEqual(3, bu.Start);
            Assert.AreEqual(10, bu.End);

            var c = new Range<int>(1, 10);
            var cu = c.Union(new Range<int>(3, 6));

            Assert.AreEqual(1, cu.Start);
            Assert.AreEqual(10, cu.End);

            var d = new Range<int>(3, 6);
            var du = d.Union(new Range<int>(1, 10));

            Assert.AreEqual(1, du.Start);
            Assert.AreEqual(10, du.End);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Range_Should_Not_Union_Null()
        {
            new Range<int>(1, 3).Union(null);
        }

        [TestMethod]
        public void Range_Should_Not_Union()
        {
            var range = new Range<int>(3, 6);
            Assert.IsNull(range.Union(new Range<int>(7, 10)));
        }
    }
}
