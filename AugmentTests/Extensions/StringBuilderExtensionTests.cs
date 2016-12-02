using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class StringBuilderExtensionTests
    {
        [TestMethod]
        public void StringBuilderExtension_AppendIf_Test()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendIf(true, "X");

            Assert.AreEqual("X", sb.ToString());

            sb.Clear();

            sb.AppendIf(false, "X", "Z");

            Assert.AreEqual("Z", sb.ToString());
        }
    }
}