using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Augment.SqlServer.Development.Parsers
{
    /// <summary>
    /// A parser that detects the type and name of a sql object from a script.
    /// </summary>
    [DebuggerDisplay("{Type,nq} {Regex,nq}")]
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
            yield return new ScriptRecognizer(SchemaTypes.Unsupported, $@"ALTER TABLE (?<tablename>{SqlNamePattern}).+ADD (CONSTRAINT )?((CHECK\s*\()|(PRIMARY KEY)|(FOREIGN KEY))", "$1 : Unnamed CONSTRAINTs are not supported");
            yield return new ScriptRecognizer(SchemaTypes.Unsupported, $@"CREATE TABLE (?<tablename>{SqlNamePattern}).+(CONSTRAINT )?((CHECK\s*\()|(PRIMARY KEY)|(FOREIGN KEY))", "$1 : Inline CONSTRAINTs are not supported");

            //yield return new ScriptRecognizer(SchemaTypes.IndexedView, $@"--\s*INDEXEDVIEW.+CREATE VIEW (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.UserPreScript, $@"--\s*PRESCRIPT (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.UserScript, $@"--\s*SCRIPT (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.UserDefinedType, $@"CREATE TYPE (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.UserDefinedType, $@"EXEC(UTE)? sp_addtype '?(?<name>{SqlNamePattern})'?");
            //yield return new ScriptRecognizer(SchemaTypes.MasterKey, $@"CREATE MASTER KEY (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Certificate, $@"CREATE CERTIFICATE (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.SymmetricKey, $@"CREATE SYMMETRIC KEY (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.PartitionFunction, $@"CREATE PARTITION FUNCTION (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.PartitionScheme, $@"CREATE PARTITION SCHEME (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.MessageType, $@"CREATE MESSAGE TYPE (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Contract, $@"CREATE CONTRACT (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.BrokerPriority, $@"CREATE BROKER PRIORITY (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Queue, $@"CREATE QUEUE (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Service, $@"CREATE SERVICE (?<name>{SqlNamePattern})");
            yield return new ScriptRecognizer(SchemaTypes.Table, $@"CREATE TABLE (?<name>{SqlNamePattern})");
            yield return new ScriptRecognizer(SchemaTypes.Trigger, $@"CREATE TRIGGER (?<name>{SqlNamePattern}) ON");
            yield return new ScriptRecognizer(SchemaTypes.Index, $@"CREATE (UNIQUE )?(((CLUSTERED)|(NONCLUSTERED)) )?INDEX (?<indname>{SqlNamePattern}) ON (?<tablename>{SqlNamePattern})", "$2.$1");
            //yield return new ScriptRecognizer(SchemaTypes.View, $@"CREATE VIEW (?<name>{SqlNamePattern})");
            yield return new ScriptRecognizer(SchemaTypes.StoredProcedure, $@"CREATE PROC(EDURE)? (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Permission, $@"GRANT (?<permission>\w+(\s*,\s*\w+)*) ON (?<name>{SqlNamePattern}) TO (?<grantee>{SqlNamePattern})", "$1 ON $2 TO $3");
            yield return new ScriptRecognizer(SchemaTypes.ForeignKey, $@"ALTER TABLE (?<tablename>{SqlNamePattern}) (WITH (NO)?CHECK )?ADD CONSTRAINT (?<name>{SqlNamePattern}) FOREIGN KEY\s*", "$1.$2");
            yield return new ScriptRecognizer(SchemaTypes.PrimaryKey, $@"ALTER TABLE (?<tablename>{SqlNamePattern}) (WITH (NO)?CHECK )?ADD CONSTRAINT (?<name>{SqlNamePattern}) PRIMARY KEY\s*", "$1.$2");
            yield return new ScriptRecognizer(SchemaTypes.UniqueKey, $@"ALTER TABLE (?<tablename>{SqlNamePattern}) (WITH (NO)?CHECK )?ADD CONSTRAINT (?<name>{SqlNamePattern}) UNIQUE\s*", "$1.$2");
            //yield return new ScriptRecognizer(SchemaTypes.Constraint, $@"ALTER TABLE (?<tablename>{SqlNamePattern}) (WITH (NO)?CHECK )?ADD ((CHECK )?CONSTRAINT)\s*\(?(?<name>{SqlNamePattern})\)?", "$1.$2");
            //yield return new ScriptRecognizer(SchemaTypes.Default, $@"ALTER TABLE (?<tablename>{SqlNamePattern}) ADD (CONSTRAINT (?<name>{SqlNamePattern}) )?DEFAULT\s*\(?.*\)?FOR (?<column>{SqlNamePattern})", "$1.$3");
            //yield return new ScriptRecognizer(SchemaTypes.Function, $@"CREATE FUNCTION (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.PrimaryXmlIndex, $@"CREATE PRIMARY XML INDEX (?<name>{SqlNamePattern}) ON (?<tablename>{SqlNamePattern})", "$2.$1");
            //yield return new ScriptRecognizer(SchemaTypes.SecondaryXmlIndex, $@"CREATE XML INDEX (?<name>{SqlNamePattern}) ON (?<tablename>{SqlNamePattern})", "$2.$1");
            //yield return new ScriptRecognizer(SchemaTypes.Login, $@"CREATE LOGIN (?<name>{SqlNamePattern})", "$1");
            //yield return new ScriptRecognizer(SchemaTypes.User, $@"CREATE USER (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Role, $@"CREATE ROLE (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Schema, $@"CREATE SCHEMA (?<name>{SqlNamePattern})");
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET ANSI_NULLS", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET QUOTED_IDENTIFIER", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET ARITHABORT", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET CONCAT_NULL_YIELDS_NULL", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET ANSI_PADDING", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET ANSI_WARNINGS", null);
            //yield return new ScriptRecognizer(SchemaTypes.Unused, $@"SET NUMERIC_ROUNDABORT", null);
            //yield return new ScriptRecognizer(SchemaTypes.Script, $@"ALTER TABLE (?<tablename>{SqlNamePattern}) (WITH (NO)?CHECK )?(?!ADD )(((CHECK )?CONSTRAINT)|(DEFAULT))\s*\(?(?<name>{SqlNamePattern})\)?", "SCRIPT $1.$2");
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

            Regex = new Regex(pattern.Replace(" ", @"\s+"), RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

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
