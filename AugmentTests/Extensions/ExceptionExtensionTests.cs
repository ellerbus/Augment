using System;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests
{
    [TestClass]
    public class ExceptionExtensionTests
    {
        [TestMethod]
        public void ExceptionExtension_ToXml_WithInstantiatedException()
        {
            var ex = new Exception("Blah!");

            var doc = ex.ToXml();

            doc.Should().BeOfType<XElement>();

            doc.Should().HaveElement("Message");

            doc.Descendants().Count().Should().Be(1);
        }

        [TestMethod]
        public void ExceptionExtension_ToXml_AsThrown()
        {
            try
            {
                throw new Exception("Blah!");
            }
            catch (Exception ex)
            {
                var doc = ex.ToXml();

                doc.Should().BeOfType<XElement>();

                doc.Should().HaveElement("Message");

                doc.Should().HaveElement("StackTrace");

                doc.Descendants().Count().Should().Be(2);
            }
        }

        [TestMethod]
        public void ExceptionExtension_ToXml_InstantiatedWithData()
        {
            var ex = new Exception("Blah!");

            ex.Data["Count"] = 99;

            var doc = ex.ToXml();

            doc.Should().BeOfType<XElement>();

            doc.Should().HaveElement("Message");

            doc.Should().HaveElement("Data");

            var data = doc.Element("Data");

            data.Descendants().Count().Should().Be(1);

            data.Should().HaveElement("Count").And.HaveValue("99");

            doc.Descendants().Count().Should().Be(3);
        }

        [TestMethod]
        public void ExceptionExtension_ToXml_InstantiatedWithInnerException()
        {
            var inner = new ArgumentException("Bar!");

            var ex = new Exception("Foo!", inner);

            var doc = ex.ToXml();

            doc.Should().BeOfType<XElement>();

            doc.Should().HaveElement("System.ArgumentException");

            var innerDoc = doc.Element("System.ArgumentException");

            doc.Descendants("Message").Count().Should().Be(2);
        }
    }
}