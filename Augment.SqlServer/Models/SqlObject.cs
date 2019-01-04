using System;
using System.Diagnostics;
using Augment.SqlServer.Parsers;
using EnsureThat;

namespace Augment.SqlServer.Models
{
    [DebuggerDisplay("{Type,nq} {OriginalName,nq}")]
    public class SqlObject : IEquatable<SqlObject>
    {
        #region Constructors

        public SqlObject(ObjectTypes type, string name, string sql)
        {
            Type = type;

            OriginalName = name;

            Identifiers = name.SplitIdentifiers();

            NormalizedName = Identifiers.ToNormalizedName();

            VerifyNaming();

            OriginalSql = sql;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{Type}: {NormalizedName}";
        }

        public bool Equals(SqlObject other)
        {
            return false;
        }

        private void VerifyNaming()
        {
            switch (Type)
            {
                case ObjectTypes.UserType:
                case ObjectTypes.TableType:
                case ObjectTypes.Table:
                case ObjectTypes.View:
                case ObjectTypes.Trigger:
                case ObjectTypes.StoredProcedure:
                case ObjectTypes.Function:
                    VerifyName(2, Identifiers, "schema.object");

                    SchemaName = Identifiers[0];
                    ObjectName = Identifiers[1];
                    break;

                case ObjectTypes.PrimaryKey:
                case ObjectTypes.UniqueKey:
                case ObjectTypes.ForeignKey:
                case ObjectTypes.Index:
                    VerifyName(3, Identifiers, "schema.parent.object");

                    SchemaName = Identifiers[0];
                    OwnerName = Identifiers[1];
                    ObjectName = Identifiers[2];
                    break;

                case ObjectTypes.SystemScript:
                case ObjectTypes.UserScript:
                    break;

                default:
                    throw Type.UnsupportedException();
            }
        }

        private void VerifyName(int length, string[] identifiers, string msg)
        {
            Ensure.That(identifiers.Length, "Identifiers", x => x.WithMessage($"Expected '{length}:{msg}' identifiers found '{identifiers.Length}' for '{Type}'"))
                .Is(length);
        }

        #endregion

        #region Properties

        /// <summary>
        ///
        /// </summary>
        public ObjectTypes Type { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string OriginalName { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string NormalizedName { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string OwnerName { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string ObjectName { get; private set; }

        /// <summary>
        ///
        /// </summary>
        private string[] Identifiers { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string OriginalSql
        {
            get { return _originalSql; }
            internal set
            {
                _originalSql = value;

                NormalizedSql = Tokenizer.Normalize(value);
            }
        }

        private string _originalSql;

        /// <summary>
        ///
        /// </summary>
        public string NormalizedSql { get; private set; }

        /// <summary>
        /// Those object impacted by changes of 'this' object
        /// </summary>
        public SqlObjectCollection Impacts { get; } = new SqlObjectCollection();

        #endregion
    }
}