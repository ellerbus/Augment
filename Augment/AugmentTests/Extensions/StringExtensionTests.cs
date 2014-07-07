using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class StringExtensionTests
    {
        private enum MyEnum
        {
            Pass,
            [System.ComponentModel.Description("Failed")]
            Fail
        }

        [TestMethod]
        public void StringExtension_IsNullOrEmpty_Test()
        {
            Assert.IsTrue("".IsNullOrEmpty());
            Assert.IsFalse("x".IsNullOrEmpty());
        }

        [TestMethod]
        public void StringExtension_IsNullOrWhitespace_Test()
        {
            Assert.IsTrue("".IsNullOrWhiteSpace());
            Assert.IsTrue(" ".IsNullOrWhiteSpace());
            Assert.IsTrue("\t".IsNullOrWhiteSpace());
            Assert.IsTrue("\n".IsNullOrWhiteSpace());
            Assert.IsTrue(" ".IsNullOrWhiteSpace());
            Assert.IsFalse("x".IsNullOrWhiteSpace());
        }

        [TestMethod]
        public void StringExtension_IsNotEmpty_Test()
        {
            Assert.IsFalse("".IsNotEmpty());
            Assert.IsTrue("x".IsNotEmpty());
        }

        [TestMethod]
        public void StringExtension_IsSameAs_Test()
        {
            Assert.IsFalse("World".IsSameAs("Hello"));
            Assert.IsTrue("HELLO".IsSameAs("Hello"));
            Assert.IsTrue("Hello".IsSameAs("Hello"));
        }

        [TestMethod]
        public void StringExtension_AssertNotNull_Test()
        {
            string actual = null;

            Assert.AreEqual("", actual.AssertNotNull());
            Assert.AreEqual("abc", actual.AssertNotNull("abc"));
        }

        [TestMethod]
        public void StringExtension_FormatArgs_Arg0_Test()
        {
            Assert.AreEqual("Hello Joe", "Hello {0}".FormatArgs("Joe"));
        }

        [TestMethod]
        public void StringExtension_FormatArgs_Arg0_Arg1_Test()
        {
            Assert.AreEqual("Hello Joe", "{0} {1}".FormatArgs("Hello", "Joe"));
        }

        [TestMethod]
        public void StringExtension_FormatArgs_Arg0_Arg1_Arg2_Test()
        {
            Assert.AreEqual("Hello Joe!", "{0} {1}{2}".FormatArgs("Hello", "Joe", '!'));
        }

        [TestMethod]
        public void StringExtension_FormatArgs_Args_Test()
        {
            Assert.AreEqual("Hello Joe 1 2 3 4", "Hello {0} {1} {2} {3} {4}".FormatArgs("Joe", 1, 2, 3, 4));
        }

        [TestMethod]
        public void StringExtension_ToEnum_Test()
        {
            Assert.AreEqual(MyEnum.Pass, "Pass".ToEnum<MyEnum>());
            Assert.AreEqual(MyEnum.Fail, "Fail".ToEnum<MyEnum>());
            Assert.AreEqual(MyEnum.Fail, "Failed".ToEnum<MyEnum>());
        }

        [TestMethod]
        public void StringExtension_Left_Test()
        {
            Assert.AreEqual("ABC", "ABCDEF".Left(3));
            Assert.AreEqual("AB", "AB".Left(3));
        }

        [TestMethod]
        public void StringExtension_GetLeftOf_Test()
        {
            string s = "abcdef";

            Assert.AreEqual("abc", s.GetLeftOf("d"));
        }

        [TestMethod]
        public void StringExtension_Right_Test()
        {
            Assert.AreEqual("DEF", "ABCDEF".Right(3));
            Assert.AreEqual("AB", "AB".Right(3));
        }

        [TestMethod]
        public void StringExtension_GetRightOf_Test()
        {
            string s = "abcdef";

            Assert.AreEqual("def", s.GetRightOf("c"));
        }

        [TestMethod]
        public void StringExtension_Join_Test()
        {
            string[] values = new[] { "1", "2" };

            Assert.AreEqual("1,2", values.Join(","));
        }
    }
}