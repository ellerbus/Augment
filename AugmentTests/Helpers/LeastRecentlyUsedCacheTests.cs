using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class LeastRecentlyUsedCacheTests
    {
        [TestMethod]
        public void LeastRecentlyUsedCache_ClearTest()
        {
            LeastRecentlyUsedCache<int, string> lru = new LeastRecentlyUsedCache<int, string>(2);

            for (int i = 0; i < 2; i++)
            {
                lru[i] = i.ToString();
            }

            lru.Clear();

            Assert.AreEqual(0, lru.Count);
        }

        [TestMethod]
        public void LeastRecentlyUsedCache_CapacityTest()
        {
            LeastRecentlyUsedCache<int, string> lru = new LeastRecentlyUsedCache<int, string>(2);

            for (int i = 0; i < 3; i++)
            {
                lru[i] = i.ToString();
            }

            Assert.AreEqual(2, lru.Count);
        }

        [TestMethod]
        public void LeastRecentlyUsedCache_ContainsTrueTest()
        {
            LeastRecentlyUsedCache<int, string> lru = new LeastRecentlyUsedCache<int, string>(2);

            for (int i = 0; i < 3; i++)
            {
                lru[i] = i.ToString();
            }

            Assert.IsTrue(lru.ContainsKey(1));
        }

        [TestMethod]
        public void LeastRecentlyUsedCache_ContainsFalseTest()
        {
            LeastRecentlyUsedCache<int, string> lru = new LeastRecentlyUsedCache<int, string>(2);

            for (int i = 0; i < 3; i++)
            {
                lru[i] = i.ToString();
            }

            Assert.IsFalse(lru.ContainsKey(0));
        }

        [TestMethod]
        public void LeastRecentlyUsedCache_LookupTest()
        {
            LeastRecentlyUsedCache<int, string> lru = new LeastRecentlyUsedCache<int, string>(2);

            for (int i = 0; i < 3; i++)
            {
                lru[i] = i.ToString();
            }

            Assert.AreEqual("1", lru[1]);
        }

        [TestMethod]
        public void LeastRecentlyUsedCache_RecentTest()
        {
            LeastRecentlyUsedCache<int, string> lru = new LeastRecentlyUsedCache<int, string>(2);

            for (int i = 0; i < 3; i++)
            {
                lru[i] = i.ToString();
            }
            
            //  since "1" is the oldest it should get removed

            lru[0] = "0";

            Assert.AreEqual("0", lru.First);
            Assert.AreEqual("2", lru.Last);
        }
    }
}
