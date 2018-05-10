using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Augment.SqlServer.Models;
using Augment.SqlServer.Parsers;

namespace Augment.SqlServer.Analyzers
{
    public interface IDatabaseAnalyzer
    {
        SqlConnection Connection { get; }

        SqlObject DropOf(SqlObject drop);

        void Drop(SqlObject rename);

        void Add(SqlObject source);

        void ApplyImpacts(SqlObject target);
    }

    public class DatabaseAnalyzer : IDatabaseAnalyzer
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

        #endregion

        #region Constructor

        public DatabaseAnalyzer(SqlObjectCollection source, SqlObjectCollection target, RegistryObjectCollection registry, SqlConnection connection)
        {
            Connection = connection;

            _source = source;
            _target = target;
            _registry = registry;

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

            //  add all but user scripts
            IEnumerable<SqlObject> sqlObjects = _adds
                .Where(x => x.Type != ObjectTypes.UserScript)
                .OrderBy(x => x.Type);

            foreach (SqlObject add in sqlObjects)
            {
                Logger.Adding(add);

                yield return add;
            }

            //  add all user scripts in name order
            IEnumerable<SqlObject> userScripts = _adds
                .Where(x => x.Type == ObjectTypes.UserScript)
                .OrderBy(x => x.NormalizedName);

            foreach (SqlObject add in userScripts)
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

        public void Add(SqlObject sqlObj)
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

        public void Drop(SqlObject sqlObj)
        {
            SqlObject drop = DropOf(sqlObj);

            _drops.Add(drop);

            _registry.Drop(drop);
        }

        public SqlObject DropOf(SqlObject sqlObj)
        {
            string sql = null;

            switch (sqlObj.Type)
            {
                case ObjectTypes.Function:
                    sql = $"drop function {sqlObj.SchemaName}.{sqlObj.ObjectName}";
                    break;

                case ObjectTypes.StoredProcedure:
                    sql = $"drop proc {sqlObj.SchemaName}.{sqlObj.ObjectName}";
                    break;

                case ObjectTypes.TableType:
                case ObjectTypes.UserType:
                    sql = $"drop type {sqlObj.SchemaName}.{sqlObj.ObjectName}";
                    break;

                case ObjectTypes.Table:
                    sql = $"drop table {sqlObj.SchemaName}.{sqlObj.ObjectName}";
                    break;

                case ObjectTypes.View:
                    sql = $"drop view {sqlObj.SchemaName}.{sqlObj.ObjectName}";
                    break;

                case ObjectTypes.Trigger:
                    sql = $"drop trigger {sqlObj.SchemaName}.{sqlObj.ObjectName}";
                    break;

                case ObjectTypes.Index:
                    sql = $"drop index {sqlObj.ObjectName} on {sqlObj.SchemaName}.{sqlObj.OwnerName}";
                    break;

                case ObjectTypes.PrimaryKey:
                case ObjectTypes.UniqueKey:
                case ObjectTypes.ForeignKey:
                    sql = $"alter table {sqlObj.SchemaName}.{sqlObj.OwnerName} drop constraint {sqlObj.ObjectName}";
                    break;

                case ObjectTypes.SystemScript:
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
                case ObjectTypes.Function:
                case ObjectTypes.StoredProcedure:
                case ObjectTypes.PrimaryKey:
                case ObjectTypes.UniqueKey:
                case ObjectTypes.ForeignKey:
                case ObjectTypes.TableType:
                case ObjectTypes.View:
                    Drop(target);
                    Add(source);
                    ApplyImpacts(target);
                    break;

                case ObjectTypes.Table:
                    TableAnalyzer tableAnalyzer = new TableAnalyzer(this);

                    tableAnalyzer.Analyze(source, target);
                    break;

                case ObjectTypes.UserType:
                    UserTypeAnalyzer typeAnalyzer = new UserTypeAnalyzer(this);

                    typeAnalyzer.Analyze(source, target);
                    break;

                case ObjectTypes.UserScript:
                    throw new InvalidOperationException("Once a UserScript has been registred it's SQL should not be modified");

                default:
                    throw source.Type.UnsupportedException();
            }

        }

        public void ApplyImpacts(SqlObject target)
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

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public SqlConnection Connection { get; private set; }

        #endregion
    }
}
