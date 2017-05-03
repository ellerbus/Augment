using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Augment.SqlServer.Data;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests.SqlServer.Data
{
    [TestClass]
    public class SqlStatementBuilderTests
    {
        #region Models

        [Table("table", Schema = "x")]
        class ObjectModel
        {
            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("main_id")]
            public int MainId { get; set; }

            [Key, Column("child_id")]
            public int ChildId { get; set; }

            [Column("model_name")]
            public string Name { get; set; }

            [DatabaseGenerated(DatabaseGeneratedOption.Computed), Column("calculated")]
            public int Calculated { get; set; }

            [Column("row_version"), Timestamp]
            public byte[] RowVersion { get; set; }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void SqlStatementBuilder_MergeOne_ShouldBe_Simple()
        {
            var sql = SqlStatementBuilder.CreateMergeOne<ObjectModel>();

            sql.Should().Be("merge x.table as tgt using (select @MainId main_id, @ChildId child_id, @Name model_name) as src on (tgt.main_id = src.main_id and tgt.child_id = src.child_id) when matched then update set tgt.model_name = src.model_name when not matched by target then insert (child_id, model_name) values (src.child_id, src.model_name) output inserted.main_id, inserted.calculated, inserted.row_version");
        }

        [TestMethod]
        public void SqlStatementBuilder_MergeMany_ShouldBe_SomewhatComplex()
        {
            var sql = SqlStatementBuilder.CreateMergeMany<ObjectModel>();

            sql.Should().Be("merge x.table as tgt using (select main_id, child_id, model_name from @items) as src on (tgt.main_id = src.main_id and tgt.child_id = src.child_id) when matched then update set tgt.model_name = src.model_name when not matched by target then insert (child_id, model_name) values (src.child_id, src.model_name) ");
        }

        #endregion

        #region Insert

        [TestMethod]
        public void SqlStatementBuilder_InsertOne_ShouldBe_Simple()
        {
            var sql = SqlStatementBuilder.CreateInsertOne<ObjectModel>();

            sql.Should().Be("insert into x.table (child_id, model_name) output inserted.main_id, inserted.calculated, inserted.row_version values (@ChildId, @Name)");
        }

        [TestMethod]
        public void SqlStatementBuilder_InsertMany_ShouldBe_SomewhatComplex()
        {
            var sql = SqlStatementBuilder.CreateInsertMany<ObjectModel>();

            sql.Should().Be("insert into x.table (child_id, model_name)  select child_id, model_name from @items");
        }

        #endregion

        #region Update

        [TestMethod]
        public void SqlStatementBuilder_UpdateOne_ShouldBe_Simple()
        {
            var sql = SqlStatementBuilder.CreateUpdateOne<ObjectModel>();

            sql.Should().Be("update x.table set model_name = @Name output inserted.calculated, inserted.row_version where main_id = @MainId and child_id = @ChildId");
        }

        [TestMethod]
        public void SqlStatementBuilder_UpdateMany_ShouldBe_SomewhatComplex()
        {
            var sql = SqlStatementBuilder.CreateUpdateMany<ObjectModel>();

            sql.Should().Be("update tgt set tgt.model_name = src.model_name  from x.table tgt inner join @items src on tgt.main_id = src.main_id and tgt.child_id = src.child_id");
        }

        #endregion

        #region Delete

        [TestMethod]
        public void SqlStatementBuilder_DeleteOne_ShouldBe_Simple()
        {
            var sql = SqlStatementBuilder.CreateDeleteOne<ObjectModel>();

            sql.Should().Be("delete from x.table where main_id = @MainId and child_id = @ChildId");
        }

        [TestMethod]
        public void SqlStatementBuilder_DeleteMany_ShouldBe_SomewhatComplex()
        {
            var sql = SqlStatementBuilder.CreateDeleteMany<ObjectModel>();

            sql.Should().Be("delete tgt from x.table tgt inner join @items src on tgt.main_id = src.main_id and tgt.child_id = src.child_id");
        }

        #endregion

        #region Select

        [TestMethod]
        public void SqlStatementBuilder_SelectOne_ShouldBe_Simple()
        {
            var sql = SqlStatementBuilder.CreateSelectOne<ObjectModel>();

            sql.Should().Be("select * from x.table where main_id = @MainId and child_id = @ChildId");
        }

        [TestMethod]
        public void SqlStatementBuilder_SelectMany_ShouldBe_Simpler()
        {
            var sql = SqlStatementBuilder.CreateSelectMany<ObjectModel>();

            sql.Should().Be("select * from x.table");
        }

        #endregion
    }
}
