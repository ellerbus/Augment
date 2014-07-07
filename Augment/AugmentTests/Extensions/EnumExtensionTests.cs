using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class EnumExtensionTests
    {
        private enum MyEnum
        {
            Pass,
            [System.ComponentModel.Description("Failed")]
            Fail
        }

        [TestMethod]
        public void EnumExtension_ToDescription_Test()
        {
            Assert.AreEqual("Pass", MyEnum.Pass.ToDescription());
            Assert.AreEqual("Failed", MyEnum.Fail.ToDescription());
        }
    }
}