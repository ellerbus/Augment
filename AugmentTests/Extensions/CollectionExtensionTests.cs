using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class CollectionExtensionTests
    {
        [TestMethod]
        public void CollectionExtension_IEnumerable_IsNullOrEmpty_Test()
        {
            IEnumerable<int> numbers = GetNumbers();

            Assert.IsFalse(numbers.IsNullOrEmpty());

            numbers = null;

            Assert.IsTrue(numbers.IsNullOrEmpty());
        }

        private IEnumerable<int> GetNumbers()
        {
            yield return 0;
            yield return 1;
        }

        [TestMethod]
        public void CollectionExtension_ICollection_IsNullOrEmpty_Test()
        {
            ICollection<int> col = new[] { 0, 1 };

            IEnumerable<int> numbers = col;

            Assert.IsFalse(numbers.IsNullOrEmpty());

            numbers = null;

            Assert.IsTrue(numbers.IsNullOrEmpty());
        }
    }
}