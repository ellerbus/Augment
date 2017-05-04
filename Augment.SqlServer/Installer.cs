using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Augment.SqlServer.Analyzers;
using Augment.SqlServer.Models;
using Augment.SqlServer.Parsers;
using Augment.SqlServer.Properties;
using EnsureThat;

namespace Augment.SqlServer
{
    public class Installer
    {
        #region Members

        [DebuggerDisplay("{Entity,nq} {Impacts,nq}")]
        class EntityImpact
        {
            public string Entity { get; set; }
            public string Impacts { get; set; }
        }

        private static readonly Regex _userScriptRegex = new Regex(@"^[0-9]{8,}_[A-Z0-9_]+$", RegexOptions.IgnoreCase);

        private SqlConnection _targetConnection;

        #endregion

        #region Constructors

        public Installer(Assembly source, SqlConnection target)
        {
            LoadAugmentSystemObjects();
            LoadSourceObjects(source);
            LoadTargetObjects(target);

            CreateImpactChain();

            OverlayTargetWithRegistry();
        }

        #endregion

        #region Object Loaders

        private void LoadAugmentSystemObjects()
        {
            ScriptParser parser = new ScriptParser();

            string script = Resources.RegistryScript;

            foreach (SqlObject so in parser.Parse(script))
            {
                Source.Add(so);
            }
        }

        private void LoadSourceObjects(Assembly assembly)
        {
            ScriptParser parser = new ScriptParser();

            IEnumerable<string> names = assembly.GetManifestResourceNames()
                .Where(x => x.EndsWithSameAs(".sql"));

            foreach (string name in names)
            {
                string file = name.Split('.').Reverse().Skip(1).Take(1).Single();

                Stream stream = assembly.GetManifestResourceStream(name);

                using (StreamReader reader = new StreamReader(stream))
                {
                    string script = reader.ReadToEnd();

                    if (_userScriptRegex.IsMatch(file))
                    {
                        SqlObject so = new SqlObject(ObjectTypes.UserScript, file, script);

                        Source.Add(so);
                    }
                    else
                    {
                        foreach (SqlObject so in parser.Parse(script))
                        {
                            Source.Add(so);
                        }
                    }
                }
            }

            Logger.Info($"Found {Source.Count} Source Objects");
        }

        private void LoadTargetObjects(SqlConnection target)
        {
            _targetConnection = target;

            string sql = Resources.DatabaseScript;

            string script = _targetConnection.Query<string>(sql).Join(Environment.NewLine + "go" + Environment.NewLine);

            ScriptParser parser = new ScriptParser();

            foreach (SqlObject so in parser.Parse(script))
            {
                Target.Add(so);
            }

            Logger.Info($"Found {Target.Count} Target Objects");
        }

        private void CreateImpactChain()
        {
            string sql = Resources.EntityImpactScript;

            IList<EntityImpact> entities = _targetConnection.Query<EntityImpact>(sql);

            foreach (EntityImpact e in entities)
            {
                SqlObject entity = Target.Find(e.Entity);
                SqlObject impacts = Target.Find(e.Impacts);

                Ensure.That(entity, "Sql Entity")
                    .WithExtraMessageOf(() => $"Missing SqlObject '{e.Entity}'")
                    .IsNotNull();

                Ensure.That(impacts, "Sql Impact")
                    .WithExtraMessageOf(() => $"Missing SqlObject '{e.Impacts}'")
                    .IsNotNull();

                entity.Impacts.Add(impacts);
            }
        }

        private void OverlayTargetWithRegistry()
        {
            bool exists = _targetConnection.ExecuteScalar<bool>("select count(1) from sys.tables where object_id = object_id('dbo.AugmentRegistry')");

            if (exists)
            {
                string sql = "select * from dbo.AugmentRegistry";

                IList<RegistryObject> registry = _targetConnection.Query<RegistryObject>(sql);

                foreach (RegistryObject regObj in registry)
                {
                    if (_userScriptRegex.IsMatch(regObj.RegistryName))
                    {
                        SqlObject sqlObj = new SqlObject(ObjectTypes.UserScript, regObj.RegistryName, regObj.SqlScript);

                        Target.Add(sqlObj);
                    }
                    else
                    {
                        SqlObject sqlObj = Target.Find(regObj);

                        if (sqlObj != null)
                        {
                            sqlObj.OriginalSql = regObj.SqlScript;
                        }
                    }

                    Registry.Add(regObj);
                }
            }
            else
            {
                //  we need to jit the registry
                foreach (SqlObject target in Target)
                {
                    Registry.Add(target);
                }

                //  the source will get added, but lets
                //  at least make sure SQL matches when
                //  changes aren't identified (ie. tables)
                foreach (SqlObject source in Source)
                {
                    Registry.Add(source);
                }
            }
        }

        #endregion

        #region Methods

        public void Install()
        {
            _targetConnection.Execute("begin transaction");

            try
            {
                DatabaseAnalyzer c = new DatabaseAnalyzer(Source, Target, Registry, _targetConnection);

                IList<SqlObject> operations = c.Analyze().ToList();

                foreach (SqlObject sqlObj in operations)
                {
                    _targetConnection.Execute(sqlObj.OriginalSql);
                }

                foreach (RegistryObject regObj in Registry.Where(x => x.IsModified))
                {
                    _targetConnection.Execute(regObj.ToMergeSql());
                }

                _targetConnection.Execute("commit tran");
            }
            catch (Exception exp)
            {
                Logger.Error(exp.ToString());
            }
        }

        #endregion

        #region Properties

        public SqlObjectCollection Source { get; } = new SqlObjectCollection();

        public SqlObjectCollection Target { get; } = new SqlObjectCollection();

        public RegistryObjectCollection Registry { get; } = new RegistryObjectCollection();

        #endregion
    }
}
