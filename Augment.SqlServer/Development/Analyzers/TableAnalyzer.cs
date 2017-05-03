using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Augment.SqlServer.Development.Models;
using Augment.SqlServer.Development.Parsers;
using Augment.SqlServer.Properties;
using Dapper;

namespace Augment.SqlServer.Development.Analyzers
{
    public class TableAnalyzer
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

        private IDatabaseAnalyzer _analyzer;

        #endregion

        #region Constructor

        public TableAnalyzer(IDatabaseAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        #endregion

        #region Analyze Table

        public void Analyze(SqlObject source, SqlObject target)
        {
            SqlObject tempObj = CreateTableTempSource(source);

            string sql = Resources.ColumnScript;

            IDictionary<string, ColumnDefinition> sourceColumns = _analyzer.Connection
                .Query<ColumnDefinition>(sql.FormatArgs(tempObj.OriginalName))
                .ToDictionary(x => x.Name);

            IDictionary<string, ColumnDefinition> targetColumns = _analyzer.Connection
                .Query<ColumnDefinition>(sql.FormatArgs(target.OriginalName))
                .ToDictionary(x => x.Name);

            if (TablesAreDifferent(sourceColumns, targetColumns))
            {
                //  script kill & fill
                //  rename existing table ZA*
                //  insert into accounting for identity, calculations, rowversions

                string tempName = AnalyzerNames.CreateForRename(source.ObjectName);

                SqlObject rename = CreateTableRename(tempName, source);

                SqlObject xfer = CreateTableTransfer(tempName, source, sourceColumns, targetColumns);

                _analyzer.Drop(rename);

                _analyzer.Add(source);

                _analyzer.Add(xfer);

                _analyzer.ApplyImpacts(target);
            }

            DropTableTempSource(tempObj);
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

            SqlObject xfer = new SqlObject(ObjectTypes.SystemScript, "data." + source.OriginalName, xferSql.ToString());

            return xfer;
        }

        private SqlObject CreateTableRename(string tempName, SqlObject source)
        {
            string renameSql = $"exec sp_rename '{source.SchemaName}.{source.ObjectName}', '{tempName}'";

            SqlObject rename = new SqlObject(ObjectTypes.SystemScript, "rename." + source.OriginalName, renameSql);

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

        private SqlObject CreateTableTempSource(SqlObject source)
        {
            string tempName = AnalyzerNames.CreateCompareName(source.ObjectName);

            string tempSql = _tempTableRegex.Replace(source.OriginalSql, "CREATE TABLE " + tempName);

            SqlObject temp = new SqlObject(ObjectTypes.Table, tempName, tempSql);

            _analyzer.Connection.Execute(tempSql);

            return temp;
        }

        private void DropTableTempSource(SqlObject tempObj)
        {
            SqlObject drop = _analyzer.DropOf(tempObj);

            _analyzer.Connection.Execute(drop.OriginalSql);
        }

        #endregion
    }
}
