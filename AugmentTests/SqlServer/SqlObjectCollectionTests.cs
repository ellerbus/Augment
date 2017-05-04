using Augment.SqlServer;
using Augment.SqlServer.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests.SqlServer
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
