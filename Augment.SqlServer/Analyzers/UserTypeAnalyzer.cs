using System.Text.RegularExpressions;
using Augment.SqlServer.Models;
using Augment.SqlServer.Parsers;
using Augment.SqlServer.Properties;

namespace Augment.SqlServer.Analyzers
{
    public class UserTypeAnalyzer
    {
        #region Members

        private static Regex _tempTypeRegex = new Regex($@"CREATE\s+TYPE\s+{ScriptRecognizer.SqlNamePattern}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private IDatabaseAnalyzer _analyzer;

        #endregion

        #region Constructor

        public UserTypeAnalyzer(IDatabaseAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        #endregion

        #region Analyze Type

        public void Analyze(SqlObject source, SqlObject target)
        {
            SqlObject tempObj = CreateTypeTempSource(source);

            string sql = Resources.UserTypeScript;

            string sourceDefinition = _analyzer.Connection.ExecuteScalar<string>(sql.FormatArgs(tempObj.ObjectName));

            string targetDefinition = _analyzer.Connection.ExecuteScalar<string>(sql.FormatArgs(target.ObjectName));

            if (sourceDefinition.IsNotSameAs(targetDefinition))
            {
                /*
                 * rename existing type to "old_type"
                 * create new_type
                 * run ALTER TABLE ALTER COLUMN using new_type
                 * run ALTER PROCEDURE using new_type
                 * run ALTER FUNCTION using new_type
                 * drop "old_type"
                 */

                string tempName = AnalyzerNames.CreateForRename(source.ObjectName);

                SqlObject rename = CreateTypeRename(tempName, source);

                _analyzer.Drop(rename);

                _analyzer.Add(source);

                _analyzer.ApplyImpacts(target);
            }

            DropTypeTempSource(tempObj);
        }

        private SqlObject CreateTypeRename(string tempName, SqlObject source)
        {
            string renameSql = $"exec sp_rename '{source.SchemaName}.{source.ObjectName}', '{tempName}', 'USERDATATYPE'";

            SqlObject rename = new SqlObject(ObjectTypes.SystemScript, "rename." + source.OriginalName, renameSql);

            return rename;
        }

        private SqlObject CreateTypeTempSource(SqlObject source)
        {
            string tempName = AnalyzerNames.CreateCompareName(source.ObjectName);

            string tempSql = _tempTypeRegex.Replace(source.OriginalSql, "CREATE TYPE " + tempName);

            SqlObject temp = new SqlObject(ObjectTypes.UserType, tempName, tempSql);

            _analyzer.Connection.Execute(tempSql);

            return temp;
        }

        private void DropTypeTempSource(SqlObject tempObj)
        {
            SqlObject drop = _analyzer.DropOf(tempObj);

            _analyzer.Connection.Execute(drop.OriginalSql);
        }

        #endregion
    }
}
