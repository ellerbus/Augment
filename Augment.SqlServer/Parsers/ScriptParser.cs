using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Augment.SqlServer.Models;

namespace Augment.SqlServer.Parsers
{
    public class ScriptParser
    {
        #region Members

        private static readonly Regex _printRegex = new Regex(@"^\s*PRINT\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static readonly Regex _goRegex = new Regex(@"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        #endregion

        #region Methods

        public IEnumerable<SqlObject> Parse(string script)
        {
            script = _printRegex.Replace(script, "");

            IEnumerable<string> batches = _goRegex.Split(script).Where(x => x.IsNotEmpty());

            foreach (string sql in batches)
            {
                bool matched = false;

                foreach (ScriptRecognizer recognizer in ScriptRecognizer.Recognizers)
                {
                    Match m = recognizer.Regex.Match(sql);

                    if (m.Success)
                    {
                        yield return CreateSqlObject(recognizer, m, sql);

                        matched = true;
                    }
                }

                if (!matched)
                {
                    throw new FormatException("Cannot Recognize SQL SchemaType: " + sql);
                }
            }
        }

        private SqlObject CreateSqlObject(ScriptRecognizer recognizer, Match m, string sql)
        {
            if (recognizer.Type == ObjectTypes.Unsupported)
            {
                throw new Exception("Unsupported SQL SchemaType: " + sql);
            }

            string name = m.Result(recognizer.NamePattern);

            SqlObject so = new SqlObject(recognizer.Type, name, sql);

            return so;
        }

        #endregion
    }
}