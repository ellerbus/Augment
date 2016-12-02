using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class TypeExtensionTests
    {
        [TestMethod]
        public void TypeExtension_GetDescription_Test()
        {
            Assert.AreEqual(
                "System.Int32",
                typeof(int).GetDescription()
                );

            Assert.AreEqual(
                "ToString()",
                typeof(int).GetMethod("ToString", new Type[] { }).GetDescription()
                );

            Assert.AreEqual(
                "CompareTo(Int32)",
                typeof(int).GetMethod("CompareTo", new Type[] { typeof(int) }).GetDescription()
                );

            Assert.AreEqual(
                "System.Collections.Generic.List<T>",
                typeof(List<>).GetDescription()
                );

            Assert.AreEqual(
                "System.Collections.Generic.Dictionary<TKey,TValue>",
                typeof(Dictionary<,>).GetDescription()
                );

            Assert.AreEqual(
                "System.Collections.Generic.List<System.Int32>",
                typeof(List<int>).GetDescription()
                );

            Assert.AreEqual(
                "System.Collections.Generic.Dictionary<System.Int32,System.Int32>",
                typeof(Dictionary<int, int>).GetDescription()
                );
        }

        [TestMethod]
        public void TypeExtension_IsNullable_Test()
        {
            Assert.IsFalse(typeof(int).IsNullable());
            Assert.IsTrue(typeof(int?).IsNullable());
            Assert.IsTrue(typeof(string).IsNullable());
            Assert.IsTrue(typeof(TypeExtensionTests).IsNullable());
        }
    }
}