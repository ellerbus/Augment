using Augment.SqlServer;
using Augment.SqlServer.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests.SqlServer
{
    [TestClass]
    public class RegistryObjectTests
    {
        [TestMethod]
        public void RegistryObject_Constructor_Should_UseSqlObject()
        {
            var so = new SqlObject(ObjectTypes.StoredProcedure, "dbo.SP", "create proc dbo.sp as");

            var ro = new RegistryObject(so);

            ro.RegistryName.Should().Be(so.NormalizedName);
            ro.SqlScript.Should().Be(so.OriginalSql);
            ro.IsModified.Should().BeTrue();
        }
    }
}
