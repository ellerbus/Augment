using Augment.SqlServer.Development;
using Augment.SqlServer.Development.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests.SqlServer.Development
{
    [TestClass]
    public class SqlObjectTests
    {
        [TestMethod]
        public void SqlObject_Constructor_Should_NormalizeTheName_WithStoredProcedures()
        {
            var so = new SqlObject(ObjectTypes.StoredProcedure, "dbo.SP", "create proc dbo.sp as");

            so.OriginalName.Should().Be("dbo.SP");
            so.NormalizedName.Should().Be("dbo.sp");
            so.NormalizedSql.Should().Be("CREATE PROC DBO . SP AS");
        }

        [TestMethod]
        public void SqlObject_OriginalSql_Should_SetNormalizedSql()
        {
            var so = new SqlObject(ObjectTypes.StoredProcedure, "dbo.SP", "create proc dbo.sp as");

            var po = new PrivateObject(so);

            po.SetProperty("OriginalSql", "create proc dbo.spa as");

            so.NormalizedSql.Should().Be("CREATE PROC DBO . SPA AS");
        }
    }
}
