using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Augment.SqlServer.Development.Models;
using Augment.SqlServer.Development.Parsers;
using Augment.SqlServer.Mapping;
using Augment.SqlServer.Properties;
using Dapper;
using EnsureThat;

namespace Augment.SqlServer.Development
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

        private static readonly Regex _userScriptRegex = new Regex("^[0-9]{8}_");

        private IDbConnection _targetConnection;

        #endregion

        #region Constructors

        static Installer()
        {
            TypeMapper.Initialize(typeof(RegistryObject));
        }

        public Installer(Assembly source, IDbConnection target)
        {
            LoadSystemObjects();
            LoadSourceObjects(source);
            LoadTargetObjects(target);

            CreateImpactChain();

            OverlayTargetWithRegistry();
        }

        #endregion

        #region Object Loaders

        private void LoadSystemObjects()
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
                        //SqlObject so = new SqlObject(SchemaTypes.UserScript, file, script);
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

        private void LoadTargetObjects(IDbConnection target)
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

            IList<EntityImpact> entities = _targetConnection.Query<EntityImpact>(sql).ToList();

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
            bool exists = _targetConnection.QueryFirst<bool>("select count(1) from sys.tables where object_id = object_id('dbo.AugmentRegistry')");

            if (exists)
            {
                string sql = "select * from dbo.AugmentRegistry";

                IList<RegistryObject> registry = _targetConnection.Query<RegistryObject>(sql).ToList();

                foreach (RegistryObject regObj in registry)
                {
                    SqlObject sqlObj = Target.Find(regObj);

                    if (sqlObj != null)
                    {
                        sqlObj.OriginalSql = regObj.SqlScript;
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
                Analyzer c = new Analyzer(Source, Target, Registry, _targetConnection);

                IList<SqlObject> operations = c.Analyze().ToList();

                foreach (SqlObject sqlObj in operations)
                {
                    _targetConnection.Execute(sqlObj.OriginalSql);
                }

                foreach (RegistryObject regObj in Registry.Where(x => x.IsModified))
                {
                    Logger.Registering(regObj);

                    _targetConnection.Execute(regObj.ToMergeSql());
                }

                _targetConnection.Execute("commit");
            }
            catch (Exception exp)
            {
                Logger.Error(exp.ToString());

                _targetConnection.Execute("rollback");
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
