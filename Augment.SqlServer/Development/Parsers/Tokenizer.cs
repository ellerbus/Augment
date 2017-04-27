using System.Collections.Generic;
using System.Linq;

namespace Augment.SqlServer.Development.Parsers
{
    public class Tokenizer
    {
        #region Members

        private static readonly string[] _keywordList = {
            "ADD",
            "ALL",
            "ALTER",
            "AND",
            "ANY",
            "AS",
            "ASC",
            "AUTHORIZATION",
            "BACKUP",
            "BEGIN",
            "BETWEEN",
            "BREAK",
            "BROWSE",
            "BULK",
            "BY",
            "CASCADE",
            "CASE",
            "CHECK",
            "CHECKPOINT",
            "CLOSE",
            "CLUSTERED",
            "COALESCE",
            "COLLATE",
            "COLUMN",
            "COMMIT",
            "COMPUTE",
            "CONSTRAINT",
            "CONTAINS",
            "CONTAINSTABLE",
            "CONTINUE",
            "CONVERT",
            "CREATE",
            "CROSS",
            "CURRENT",
            "CURRENT_DATE",
            "CURRENT_TIME",
            "CURRENT_TIMESTAMP",
            "CURRENT_USER",
            "CURSOR",
            "DATABASE",
            "DBCC",
            "DEALLOCATE",
            "DECLARE",
            "DEFAULT",
            "DELETE",
            "DENY",
            "DESC",
            "DISK",
            "DISTINCT",
            "DISTRIBUTED",
            "DOUBLE",
            "DROP",
            "DUMP",
            "ELSE",
            "END",
            "ERRLVL",
            "ESCAPE",
            "EXCEPT",
            "EXEC",
            "EXECUTE",
            "EXISTS",
            "EXIT",
            "EXTERNAL",
            "FETCH",
            "FILE",
            "FILLFACTOR",
            "FOR",
            "FOREIGN",
            "FREETEXT",
            "FREETEXTTABLE",
            "FROM",
            "FULL",
            "FUNCTION",
            "GOTO",
            "GRANT",
            "GROUP",
            "HAVING",
            "HOLDLOCK",
            "IDENTITY",
            "IDENTITY_INSERT",
            "IDENTITYCOL",
            "IF",
            "IN",
            "INDEX",
            "INNER",
            "INSERT",
            "INTERSECT",
            "INTO",
            "IS",
            "JOIN",
            "KEY",
            "KILL",
            "LEFT",
            "LIKE",
            "LINENO",
            "LOAD",
            "MERGE",
            "NATIONAL",
            "NOCHECK",
            "NONCLUSTERED",
            "NOT",
            "NULL",
            "NULLIF",
            "OF",
            "OFF",
            "OFFSETS",
            "ON",
            "OPEN",
            "OPENDATASOURCE",
            "OPENQUERY",
            "OPENROWSET",
            "OPENXML",
            "OPTION",
            "OR",
            "ORDER",
            "OUTER",
            "OVER",
            "PERCENT",
            "PIVOT",
            "PLAN",
            "PRECISION",
            "PRIMARY",
            "PRINT",
            "PROC",
            "PROCEDURE",
            "PUBLIC",
            "RAISERROR",
            "READ",
            "READTEXT",
            "RECONFIGURE",
            "REFERENCES",
            "REPLICATION",
            "RESTORE",
            "RESTRICT",
            "RETURN",
            "REVERT",
            "REVOKE",
            "RIGHT",
            "ROLLBACK",
            "ROWCOUNT",
            "ROWGUIDCOL",
            "RULE",
            "SAVE",
            "SCHEMA",
            "SECURITYAUDIT",
            "SELECT",
            "SEMANTICKEYPHRASETABLE",
            "SEMANTICSIMILARITYDETAILSTABLE",
            "SEMANTICSIMILARITYTABLE",
            "SESSION_USER",
            "SET",
            "SETUSER",
            "SHUTDOWN",
            "SOME",
            "STATISTICS",
            "SYSTEM_USER",
            "TABLE",
            "TABLESAMPLE",
            "TEXTSIZE",
            "THEN",
            "TO",
            "TOP",
            "TRAN",
            "TRANSACTION",
            "TRIGGER",
            "TRUNCATE",
            "TRY_CONVERT",
            "TSEQUAL",
            "UNION",
            "UNIQUE",
            "UNPIVOT",
            "UPDATE",
            "UPDATETEXT",
            "USE",
            "USER",
            "VALUES",
            "VARYING",
            "VIEW",
            "WAITFOR",
            "WHEN",
            "WHERE",
            "WHILE",
            "WITH",
            "WITHINGROUP",
            "WRITETEXT"
           };

        private HashSet<string> _keywords = new HashSet<string>(_keywordList);

        #endregion

        #region Methods

        public static string Normalize(string sql)
        {
            Tokenizer t = new Tokenizer();

            return t.Tokenize(sql).Select(x => x.Text).Join(" ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<Token> Tokenize(string sql)
        {
            Scanner = new Scanner(sql.Trim());

            while (Scanner.HasText)
            {
                char c1 = Scanner.Peek(1);
                char c2 = Scanner.Peek(2);

                if (char.IsWhiteSpace(c1))
                {
                    Scanner.Match();

                    continue;
                }

                if (Scanner.CanMatchSequence("--"))
                {
                    yield return GetSingleLineComment();
                }
                else if (Scanner.CanMatchSequence("/*"))
                {
                    yield return GetMultiLineComment();
                }
                else if (c1 == '"')
                {
                    yield return GetEnclosedWord('"');
                }
                else if (c1 == '[')
                {
                    yield return GetEnclosedWord(']');
                }
                else if (c1 == '\'')
                {
                    yield return GetStringLiteral(false);
                }
                else if (Scanner.CanMatchSequence("N'"))
                {
                    yield return GetStringLiteral(true);
                }
                else if (Scanner.CanMatchSequence("0X"))
                {
                    yield return GetBinaryLiteral();
                }
                else if (Scanner.CanMatchAny("0123456789"))
                {
                    yield return GetNumericLiteral();
                }
                else if (c1 == '.' && char.IsDigit(c2))
                {
                    yield return GetNumericLiteral();
                }
                else if (Scanner.CanMatchAny("ABCDEFGHIJKLMNOPQRSTUVWXYZ_"))
                {
                    yield return GetWord();
                }
                else if (Scanner.CanMatchAny("$#@:"))
                {
                    yield return GetVariable(c1);
                }
                else
                {
                    Token token = new Token(TokenTypes.Symbol, Scanner, Scanner.Peek(1).ToString());

                    Scanner.MatchAny("`~!%^&*()-_=+{[}]|\\:;\"'<,>.?/");

                    yield return token;
                }
            }
        }

        private Token GetEnclosedWord(char endsWith)
        {
            Token token = CreateToken(TokenTypes.EnclosedWord);

            Scanner.Match();

            while (Scanner.HasText && Scanner.CannotMatch(endsWith))
            {
                Scanner.Match();
            }

            Scanner.Match(endsWith);

            FillText(token);

            return token;
        }

        private Token GetWord()
        {
            Token token = CreateToken(TokenTypes.Word);

            while (Scanner.CanMatchAny("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"))
            {
                Scanner.Match();
            }

            FillText(token);

            if (_keywords.Contains(token.Text))
            {
                token.Type = TokenTypes.Keyword;
            }

            return token;
        }

        private Token GetVariable(char startsWith)
        {
            Token token = CreateToken(TokenTypes.Variable);

            Scanner.Match(startsWith);

            while (Scanner.CanMatchAny("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"))
            {
                Scanner.Match();
            }

            FillText(token);

            return token;
        }

        private Token GetBinaryLiteral()
        {
            Token token = CreateToken(TokenTypes.BinaryLiteral);

            Scanner.MatchSequence("0X");

            while (Scanner.CanMatchAny("0123456789ABCDEF"))
            {
                Scanner.Match();
            }

            FillText(token);

            return token;
        }

        private Token GetNumericLiteral()
        {
            Token token = CreateToken(TokenTypes.NumericLiteral);

            while (Scanner.CanMatchAny("0123456789"))
            {
                Scanner.Match();
            }

            if (Scanner.CanMatch('.'))
            {
                Scanner.Match('.');
            }

            while (Scanner.CanMatchAny("0123456789"))
            {
                Scanner.Match();
            }

            if (Scanner.CanMatch('E'))
            {
                Scanner.Match('E');

                if (Scanner.CanMatchAny("+-"))
                {
                    Scanner.Match();
                }

                Scanner.MatchAny("0123456789");

                while (Scanner.CanMatchAny("0123456789"))
                {
                    Scanner.Match();
                }
            }

            FillText(token);

            return token;
        }

        private Token GetSingleLineComment()
        {
            Token token = CreateToken(TokenTypes.Comment);

            Scanner.MatchSequence("--");

            while (Scanner.HasText && Scanner.CannotMatchAny("\r\n"))
            {
                Scanner.Match();
            }

            FillText(token);

            return token;
        }

        private Token GetMultiLineComment()
        {
            Token token = CreateToken(TokenTypes.Comment);

            Scanner.MatchSequence("/*");

            while (Scanner.HasText && Scanner.CannotMatchSequence("*/"))
            {
                Scanner.Match();
            }

            Scanner.MatchSequence("*/");

            FillText(token);

            return token;
        }

        private Token GetStringLiteral(bool unicode)
        {
            Token token = CreateToken(TokenTypes.StringLiteral);

            if (unicode)
            {
                Scanner.MatchSequence("N'");
            }
            else
            {
                Scanner.Match('\'');
            }

            while (Scanner.HasText)
            {
                char c1 = Scanner.Peek(1);
                char c2 = Scanner.Peek(2);

                if (c1 == '\'' && c2 == '\'')
                {
                    Scanner.Match();
                }
                else if (c1 == '\'' && c2 != '\'')
                {
                    break;
                }

                Scanner.Match();
            }

            Scanner.Match('\'');

            FillText(token);

            return token;
        }

        private Token CreateToken(TokenTypes type)
        {
            Token token = new Token(type, Scanner);

            return token;
        }

        private void FillText(Token token)
        {
            int length = Scanner.Position - token.Start;

            token.Text = Scanner.Text.Substring(token.Start, length);
        }

        #endregion

        #region Properties

        private Scanner Scanner { get; set; }

        #endregion
    }
}


