using Augment.SqlServer.Development;
using Augment.SqlServer.Development.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests.SqlServer.Development
{
    [TestClass]
    public class SqlObjectCollectionTests
    {
        [TestMethod]
        public void SqlObjectCollection_Should_ByNormalizedName()
        {
            var so = new SqlObject(ObjectTypes.StoredProcedure, "dbo.SP", "create proc dbo.sp as");

            var col = new SqlObjectCollection() { so };

            col.Contains(so).Should().BeTrue();
        }
    }
}
