using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Augment.SqlServer.Development.Parsers
{
    /// <summary>
    /// A parser that detects the type and name of a sql object from a script.
    /// </summary>
    public class ScriptRecognizer
    {
        #region Static Members

        /// <summary>
        /// Matches a SQL name in the form [a].[b].[c], or "a"."b"."c" or a.b.c (or any combination)
        /// Also: TYPE :: sqlname for global scoping
        /// </summary>
        public const string SqlNamePattern = @"([\w\d]+\s*::\s*)?((\[[^\]]+\]|[\w\d]+)\.){0,2}((\[[^\]]+\]|[\w\d]+))";

        /// <summary>
        /// Initializes the list of SQL parsers.
        /// </summary>
        private static IEnumerable<ScriptRecognizer> CreateRecognizers()
        {
            yield return new ScriptRecognizer(SchemaTypes.Unsupported, $@"ALTER\s+TABLE\s+(?<tablename>{SqlNamePattern}).+ADD\s+(CONSTRAINT\s+)?((CHECK\s*\()|(PRIMARY KEY)|(FOREIGN KEY))", "$1 : Unnamed CONSTRAINTs are not supported");
            yield return new ScriptRecognizer(SchemaTypes.Unsupported, $@"CREATE\s+TABLE\s+(?<tablename>{SqlNamePattern}).+(CONSTRAINT\s+)?((CHECK\s*\()|(PRIMARY KEY)|(FOREIGN KEY))", "$1 : Inline CONSTRAINTs are not supported");

            //yield return new ScriptRecognizer(SchemaTypes.IndexedView, $@"--\s*INDEXEDVIEW.+CREATE\s+VIEW\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.UserPreScript, $@"--\s*PRESCRIPT\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.UserScript, $@"--\s*SCRIPT\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.UserDefinedType, $@"CREATE\s+TYPE\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.UserDefinedType, $@"EXEC(UTE)?\s+sp_addtype\s+'?(?<name>{SqlNamePattern})'?");
            //yield return new ScriptRecognizer(SchemaTypes.MasterKey, $@"CREATE\s+MASTER\s+KEY\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Certificate, $@"CREATE\s+CERTIFICATE\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.SymmetricKey, $@"CREATE\s+SYMMETRIC\s+KEY\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.PartitionFunction, $@"CREATE\s+PARTITION\s+FUNCTION\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.PartitionScheme, $@"CREATE\s+PARTITION\s+SCHEME\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.MessageType, $@"CREATE\s+MESSAGE TYPE\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Contract, $@"CREATE\s+CONTRACT\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.BrokerPriority, $@"CREATE\s+BROKER\s+PRIORITY\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Queue, $@"CREATE\s+QUEUE\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Service, $@"CREATE\s+SERVICE\s+(?<name>{SqlNamePattern})");
            yield return new ScriptRecognizer(SchemaTypes.Table, $@"CREATE\s+TABLE\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Trigger, $@"CREATE\s+TRIGGER\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Index, $@"CREATE\s+(UNIQUE\s+)?(((CLUSTERED)|(NONCLUSTERED))\s+)?INDEX\s+(?<indname>{SqlNamePattern})\s+ON\s+(?<tablename>{SqlNamePattern})", "$2.$1");
            //yield return new ScriptRecognizer(SchemaTypes.View, $@"CREATE\s+VIEW\s+(?<name>{SqlNamePattern})");
            yield return new ScriptRecognizer(SchemaTypes.StoredProcedure, $@"CREATE\s+PROC(EDURE)?\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Permission, $@"GRANT\s+(?<permission>\w+(\s*,\s*\w+)*)\s+ON\s+(?<name>{SqlNamePattern})\s+TO\s+(?<grantee>{SqlNamePattern})", "$1 ON $2 TO $3");
            yield return new ScriptRecognizer(SchemaTypes.PrimaryKey, $@"ALTER\s+TABLE\s+(?<tablename>{SqlNamePattern})\s+(WITH\s+(NO)?CHECK\s+)?ADD\s+CONSTRAINT\s*\(?(?<name>{SqlNamePattern})\)?\s+PRIMARY\s+", "$1.$2");
            yield return new ScriptRecognizer(SchemaTypes.UniqueKey, $@"ALTER\s+TABLE\s+(?<tablename>{SqlNamePattern})\s+(WITH\s+(NO)?CHECK\s+)?ADD\s+CONSTRAINT\s*\(?(?<name>{SqlNamePattern})\)?\s+UNIQUE\s+", "$1.$2");
            yield return new ScriptRecognizer(SchemaTypes.ForeignKey, $@"ALTER\s+TABLE\s+(?<tablename>{SqlNamePattern})\s+(WITH\s+(NO)?CHECK\s+)?ADD\s+CONSTRAINT\s*\(?(?<name>{SqlNamePattern})\)?\s+FOREIGN\s+KEY\s*\(?(?<name>{SqlNamePattern})\)?", "$1.$2");
            //yield return new ScriptRecognizer(SchemaTypes.Constraint, $@"ALTER\s+TABLE\s+(?<tablename>{SqlNamePattern})\s+(WITH\s+(NO)?CHECK\s+)?ADD\s+((CHECK\s+)?CONSTRAINT)\s*\(?(?<name>{SqlNamePattern})\)?", "$1.$2");
            //yield return new ScriptRecognizer(SchemaTypes.Default, $@"ALTER\s+TABLE\s+(?<tablename>{SqlNamePattern})\s+ADD\s+(CONSTRAINT\s+(?<name>{SqlNamePattern})\s+)?DEFAULT\s*\(?.*\)?FOR\s+(?<column>{SqlNamePattern})", "$1.$3");
            //yield return new ScriptRecognizer(SchemaTypes.Function, $@"CREATE\s+FUNCTION\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.PrimaryXmlIndex, $@"CREATE\s+PRIMARY\s+XML\s+INDEX\s+(?<name>{SqlNamePattern})\s+ON\s+(?<tablename>{SqlNamePattern})", "$2.$1");
            //yield return new ScriptRecognizer(SchemaTypes.SecondaryXmlIndex, $@"CREATE\s+XML\s+INDEX\s+(?<name>{SqlNamePattern})\s+ON\s+(?<tablename>{SqlNamePattern})", "$2.$1");
            //yield return new ScriptRecognizer(SchemaTypes.Login, $@"CREATE\s+LOGIN\s+(?<name>{SqlNamePattern})", "$1");
            //yield return new ScriptRecognizer(SchemaTypes.User, $@"CREATE\s+USER\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Role, $@"CREATE\s+ROLE\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Schema, $@"CREATE\s+SCHEMA\s+(?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET\s+ANSI_NULLS", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET\s+QUOTED_IDENTIFIER", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET\s+ARITHABORT", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET\s+CONCAT_NULL_YIELDS_NULL", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET\s+ANSI_PADDING", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET\s+ANSI_WARNINGS", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET\s+NUMERIC_ROUNDABORT", null);
            //yield return new ScriptRecognizer(SchemaTypes.Script, $@"ALTER\s+TABLE\s+(?<tablename>{SqlNamePattern})\s+(WITH\s+(NO)?CHECK\s+)?(?!ADD\s+)(((CHECK\s+)?CONSTRAINT)|(DEFAULT))\s*\(?(?<name>{SqlNamePattern})\)?", "SCRIPT $1.$2");
            //yield return new ScriptRecognizer(SchemaTypes.AutoProc, AutoProc.AutoProcRegexString, "$0");
            //// make sure that they are sorted in the order of likelihood
            //parsers.Sort((p1, p2) => p1.SchemaTypes.CompareTo(p2.SchemaTypes);
        }

        private static int CanEmbedDdl(SchemaTypes type)
        {
            switch (type)
            {
                case SchemaTypes.StoredProcedure:
                    return 0;
            }

            return 1;
        }

        static ScriptRecognizer()
        {
            Recognizers = CreateRecognizers()
                .OrderBy(x => CanEmbedDdl(x.Type))
                .ThenBy(x => x.Type)
                .ToList();
        }

        public static IReadOnlyList<ScriptRecognizer> Recognizers { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a parser that detects a type from a pattern
        /// </summary>
        /// <param name="type">The type represented by the pattern</param>
        /// <param name="pattern">The pattern to detect</param>
        /// <param name="namePattern">The string used to generate the resulting name</param>
        public ScriptRecognizer(SchemaTypes type, string pattern, string namePattern = "$1")
        {
            Type = type;

            Regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

            NamePattern = namePattern;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The corresponding object type
        /// </summary>
        public SchemaTypes Type { get; private set; }

        /// <summary>
        /// Regex for matching
        /// </summary>
        public Regex Regex { get; private set; }

        /// <summary>
        /// The string used to generate the name from the match
        /// </summary>
        public string NamePattern { get; private set; }

        #endregion
    }
}
