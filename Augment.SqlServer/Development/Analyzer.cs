using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Augment.SqlServer.Development.Models;
using Augment.SqlServer.Development.Parsers;
using Augment.SqlServer.Properties;
using Dapper;

namespace Augment.SqlServer.Development
{
    public class Analyzer
    {
        #region Members

        class ColumnDefinition
        {
            public string Name { get; set; }
            public string Definition { get; set; }
            public bool IsIdentity { get { return Definition.Contains(" identity("); } }
            public bool IsReadOnly { get { return Definition.Contains("timestamp") || Definition.Contains("rowversion") || Definition.StartsWithSameAs("as "); } }
        }

        private static Regex _tempTableRegex = new Regex($@"CREATE\s+TABLE\s+{ScriptRecognizer.SqlNamePattern}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private SqlObjectCollection _drops;
        private SqlObjectCollection _adds;
        private SqlObjectCollection _source;
        private SqlObjectCollection _target;
        private RegistryObjectCollection _registry;
        private IDbConnection _connection;

        #endregion

        #region Constructor

        public Analyzer(SqlObjectCollection source, SqlObjectCollection target, RegistryObjectCollection registry, IDbConnection connection)
        {
            _source = source;
            _target = target;
            _registry = registry;
            _connection = connection;

            _drops = new SqlObjectCollection();
            _adds = new SqlObjectCollection();
        }

        #endregion

        #region Methods

        public IEnumerable<SqlObject> Analyze()
        {
            Logger.Info("Analyzing Differences...");

            FindDrops();

            FindAdds();

            AnalyzeDifferences();

            foreach (SqlObject drop in _drops.OrderByDescending(x => x.Type))
            {
                Logger.Dropping(drop);

                yield return drop;
            }

            foreach (SqlObject add in _adds.OrderBy(x => x.Type))
            {
                Logger.Adding(add);

                yield return add;
            }
        }

        #endregion

        #region Adds

        private void FindAdds()
        {
            //  in source not in target
            foreach (SqlObject src in _source)
            {
                if (!_target.Contains(src))
                {
                    Add(src);
                }
            }
        }

        private void Add(SqlObject sqlObj)
        {
            _adds.Add(sqlObj);

            _registry.Add(sqlObj);
        }

        #endregion

        #region Drops

        private void FindDrops()
        {
            //  in target not in source
            foreach (SqlObject tgt in _target)
            {
                if (!_source.Contains(tgt))
                {
                    Drop(tgt);
                }
            }
        }

        private void Drop(SqlObject sqlObj)
        {
            SqlObject drop = DropOf(sqlObj);

            _drops.Add(drop);

            _registry.Drop(drop);
        }

        private SqlObject DropOf(SqlObject sqlObj)
        {
            string sql = null;

            switch (sqlObj.Type)
            {
                case SchemaTypes.StoredProcedure:
                    sql = $"drop proc {sqlObj.SchemaName}.{sqlObj.ObjectName}";
                    break;

                case SchemaTypes.Table:
                    sql = $"drop table {sqlObj.SchemaName}.{sqlObj.ObjectName}";
                    break;

                case SchemaTypes.Trigger:
                    sql = $"drop trigger {sqlObj.SchemaName}.{sqlObj.ObjectName}";
                    break;

                case SchemaTypes.Index:
                    sql = $"drop index {sqlObj.ObjectName} on {sqlObj.SchemaName}.{sqlObj.OwnerName}";
                    break;

                case SchemaTypes.PrimaryKey:
                case SchemaTypes.UniqueKey:
                case SchemaTypes.ForeignKey:
                    sql = $"alter table {sqlObj.SchemaName}.{sqlObj.OwnerName} drop constraint {sqlObj.ObjectName}";
                    break;

                case SchemaTypes.SystemScript:
                    sql = sqlObj.OriginalSql;
                    break;

                default:
                    throw sqlObj.Type.UnsupportedException();
            }

            return new SqlObject(sqlObj.Type, sqlObj.OriginalName, sql);
        }

        #endregion

        #region Diffs

        private void AnalyzeDifferences()
        {
            //  in target not in source
            foreach (SqlObject target in _target)
            {
                AnalyzeDifferences(target);
            }
        }

        private void AnalyzeDifferences(SqlObject target)
        {
            if (_source.Contains(target))
            {
                Logger.Info($"Analyzing {target.ToString()}");

                //  has the SQL changed
                SqlObject source = _source.Find(target);

                if (source.NormalizedSql.IsNotSameAs(target.NormalizedSql))
                {
                    ApplyDifferences(source, target);
                }
            }
        }

        private void ApplyDifferences(SqlObject source, SqlObject target)
        {
            switch (source.Type)
            {
                case SchemaTypes.StoredProcedure:
                case SchemaTypes.PrimaryKey:
                case SchemaTypes.UniqueKey:
                case SchemaTypes.ForeignKey:
                    Drop(target);
                    Add(source);
                    ApplyImpacts(target);
                    break;

                case SchemaTypes.Table:
                    AnalyzeTable(source, target);
                    break;

                default:
                    throw source.Type.UnsupportedException();
            }

        }

        private void ApplyImpacts(SqlObject target)
        {
            foreach (SqlObject impacted in target.Impacts)
            {
                SqlObject source = _source.Find(impacted);

                if (source != null)
                {
                    ApplyDifferences(source, impacted);
                }
            }
        }

        private void AnalyzeTable(SqlObject source, SqlObject target)
        {
            SqlObject tempObj = CreateTempSource(source);

            string sql = Resources.ColumnScript;

            IDictionary<string, ColumnDefinition> sourceColumns = _connection
                .Query<ColumnDefinition>(sql.FormatArgs(tempObj.OriginalName))
                .ToDictionary(x => x.Name);

            IDictionary<string, ColumnDefinition> targetColumns = _connection
                .Query<ColumnDefinition>(sql.FormatArgs(target.OriginalName))
                .ToDictionary(x => x.Name);

            if (TablesAreDifferent(sourceColumns, targetColumns))
            {
                //  script kill & fill
                //  rename existing table ZA*
                //  insert into accounting for identity, calculations, rowversions

                string tempName = $"ZD{IdGenerator.Random()}_{source.ObjectName}";

                SqlObject rename = CreateTableRename(tempName, source);

                SqlObject xfer = CreateTableTransfer(tempName, source, sourceColumns, targetColumns);

                Drop(rename);

                Add(source);

                Add(xfer);

                ApplyImpacts(target);
            }

            DropTempSource(tempObj);
        }

        private SqlObject CreateTableTransfer(string tempName, SqlObject source, IDictionary<string, ColumnDefinition> sourceColumns, IDictionary<string, ColumnDefinition> targetColumns)
        {
            IList<string> columns = sourceColumns.Where(x => !x.Value.IsReadOnly).Select(x => x.Key)
                .Intersect(targetColumns.Select(x => x.Key))
                .ToList();

            string tableName = $"{source.SchemaName}.{source.ObjectName}";

            bool hasIdentity = sourceColumns.Any(x => x.Value.IsIdentity);

            StringBuilder xferSql = new StringBuilder();

            if (hasIdentity)
            {
                xferSql.Append($"set identity_insert {tableName} on").AppendLine().AppendLine();
            }

            xferSql.Append($"insert into {tableName}").AppendLine()
                .Append("      (").Append(columns.Join(", ")).Append(")").AppendLine()
                .Append("select ").Append(columns.Join(", ")).AppendLine()
                .Append("from   dbo.").Append(tempName)
                .AppendLine().AppendLine()
                .Append($"drop table dbo.{tempName}").AppendLine().AppendLine();

            if (hasIdentity)
            {
                xferSql.Append($"set identity_insert {tableName} off").AppendLine().AppendLine();
            }

            SqlObject xfer = new SqlObject(SchemaTypes.SystemScript, "data." + source.OriginalName, xferSql.ToString());

            return xfer;
        }

        private SqlObject CreateTableRename(string tempName, SqlObject source)
        {
            string renameSql = $"exec sp_rename '{source.SchemaName}.{source.ObjectName}', '{tempName}'";

            SqlObject rename = new SqlObject(SchemaTypes.SystemScript, "rename." + source.OriginalName, renameSql);

            return rename;
        }

        private bool TablesAreDifferent(IDictionary<string, ColumnDefinition> sourceColumns, IDictionary<string, ColumnDefinition> targetColumns)
        {
            //  requires tighter analysis 
            //  if this is the first comparison and the object already exists
            //  the SQL gen'd by the script contains more parens than the original
            //  scripts - once registered we can rely more on the SQL comparison
            //  but this is still performed
            if (sourceColumns.Count == targetColumns.Count)
            {
                IDictionary<string, ColumnDefinition> source = new Dictionary<string, ColumnDefinition>(sourceColumns);
                IDictionary<string, ColumnDefinition> target = new Dictionary<string, ColumnDefinition>(targetColumns);

                foreach (string name in source.Keys.ToList())
                {
                    ColumnDefinition src = source[name];
                    ColumnDefinition tgt = null;

                    if (target.TryGetValue(name, out tgt))
                    {
                        if (src.Definition.IsSameAs(tgt.Definition))
                        {
                            source.Remove(name);
                            target.Remove(name);
                        }
                    }
                }

                return source.Count > 0 || target.Count > 0;
            }

            return true;
        }

        private SqlObject CreateTempSource(SqlObject source)
        {
            string tempName = $"dbo.ZC{IdGenerator.Random()}_{ source.ObjectName}";

            string tempSql = _tempTableRegex.Replace(source.OriginalSql, "CREATE TABLE " + tempName);

            SqlObject temp = new SqlObject(SchemaTypes.Table, tempName, tempSql);

            _connection.Execute(tempSql);

            return temp;
        }

        private void DropTempSource(SqlObject tempObj)
        {
            SqlObject drop = DropOf(tempObj);

            _connection.Execute(drop.OriginalSql);
        }

        #endregion
    }
}
