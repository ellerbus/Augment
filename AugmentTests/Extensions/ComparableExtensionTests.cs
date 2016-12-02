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

        [TestMethod]
        public void ComparableExtensions_IsIn_Test()
        {
            int i = 5;

            Assert.IsFalse(i.IsIn(4, 6));
            Assert.IsTrue(i.IsIn(5, 6));
            Assert.IsTrue(i.IsIn(6, 5));
            Assert.IsFalse(i.IsIn(6, 4));
        }
    }
}