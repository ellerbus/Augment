//using System.Collections.Generic;
//using System.Linq;
//using Augment.SqlServer.Mapping;

//namespace Augment.SqlServer.Data
//{
//    public static class SqlStatementBuilder
//    {
//        #region Members

//        private static IDictionary<string, string> _cache = new Dictionary<string, string>();

//        #endregion

//        #region Merge

//        public static string CreateMergeOne<TEntity>()
//        {
//            lock (_cache)
//            {
//                string key = "M1:" + typeof(TEntity).FullName;

//                string merge = null;

//                if (!_cache.TryGetValue(key, out merge))
//                {
//                    ObjectMap map = ObjectMap.Create<TEntity>();

//                    string keys = map.Properties
//                        .Where(x => x.IsPrimaryKey)
//                        .Select(x => $"tgt.{x.ColumnName} = src.{x.ColumnName}")
//                        .Join(" and ");

//                    string selects = map.Properties
//                        .Where(x => x.IsForMerge)
//                        .Select(x => $"@{x.Name} {x.ColumnName}")
//                        .Join(", ");

//                    string sets = map.Properties
//                        .Where(x => x.IsForUpdate)
//                        .Select(x => $"tgt.{x.ColumnName} = src.{x.ColumnName}")
//                        .Join(", ");

//                    string inserts = map.Properties
//                        .Where(x => x.IsForInsert)
//                        .Select(x => $"{x.ColumnName}")
//                        .Join(", ");

//                    string values = map.Properties
//                        .Where(x => x.IsForInsert)
//                        .Select(x => $"src.{x.ColumnName}")
//                        .Join(", ");

//                    string outputs = map.OutputColumns
//                        .Select(x => $"inserted.{x.ColumnName}")
//                        .Join(", ");

//                    string table = $"merge {map.FullName} as tgt";

//                    string select = $"using (select {selects}) as src on ({keys})";

//                    string update = $"when matched then update set {sets}";

//                    string insert = $"when not matched by target then insert ({inserts}) values ({values})";

//                    string output = "";

//                    if (outputs.IsNotEmpty())
//                    {
//                        output = $"output {outputs}";
//                    }

//                    merge = $"{table} {select} {update} {insert} {output};";

//                    _cache.Add(key, merge);
//                }

//                return merge;
//            }
//        }

//        public static string CreateMergeMany<TEntity>()
//        {
//            lock (_cache)
//            {
//                string key = "M#:" + typeof(TEntity).FullName;

//                string merge = null;

//                if (!_cache.TryGetValue(key, out merge))
//                {
//                    ObjectMap map = ObjectMap.Create<TEntity>();

//                    string keys = map.Properties
//                        .Where(x => x.IsPrimaryKey)
//                        .Select(x => $"tgt.{x.ColumnName} = src.{x.ColumnName}")
//                        .Join(" and ");

//                    string selects = map.Properties
//                        .Where(x => x.IsForMerge)
//                        .Select(x => $"{x.ColumnName}")
//                        .Join(", ");

//                    string sets = map.Properties
//                        .Where(x => x.IsForUpdate)
//                        .Select(x => $"tgt.{x.ColumnName} = src.{x.ColumnName}")
//                        .Join(", ");

//                    string inserts = map.Properties
//                        .Where(x => x.IsForInsert)
//                        .Select(x => $"{x.ColumnName}")
//                        .Join(", ");

//                    string values = map.Properties
//                        .Where(x => x.IsForInsert)
//                        .Select(x => $"src.{x.ColumnName}")
//                        .Join(", ");

//                    string outputs = map.OutputColumns
//                        .Select(x => $"inserted.{x.ColumnName}")
//                        .Join(", ");

//                    string table = $"merge {map.FullName} as tgt";

//                    string select = $"using (select {selects} from @items) as src on ({keys})";

//                    string update = $"when matched then update set {sets}";

//                    string insert = $"when not matched by target then insert ({inserts}) values ({values})";

//                    string output = "";

//                    if (outputs.IsNotEmpty())
//                    {
//                        //  TODO    output = $"output {outputs}";
//                    }

//                    merge = $"{table} {select} {update} {insert} {output};";

//                    _cache.Add(key, merge);
//                }

//                return merge;
//            }
//        }

//        #endregion

//        #region Insert

//        public static string CreateInsertOne<TEntity>()
//        {
//            lock (_cache)
//            {
//                string key = "I1:" + typeof(TEntity).FullName;

//                string insert = null;

//                if (!_cache.TryGetValue(key, out insert))
//                {
//                    ObjectMap map = ObjectMap.Create<TEntity>();

//                    string inserts = map.Properties
//                        .Where(x => x.IsForInsert)
//                        .Select(x => $"{x.ColumnName}")
//                        .Join(", ");

//                    string values = map.Properties
//                        .Where(x => x.IsForInsert)
//                        .Select(x => $"@{x.Name}")
//                        .Join(", ");

//                    string outputs = map.OutputColumns
//                        .Select(x => $"inserted.{x.ColumnName}")
//                        .Join(", ");

//                    string output = "";

//                    if (outputs.IsNotEmpty())
//                    {
//                        output = $"output {outputs}";
//                    }

//                    insert = $"insert into {map.FullName} ({inserts}) {output} values ({values})";

//                    _cache.Add(key, insert);
//                }

//                return insert;
//            }
//        }


//        public static string CreateInsertMany<TEntity>()
//        {
//            lock (_cache)
//            {
//                string key = "I#:" + typeof(TEntity).FullName;

//                string insert = null;

//                if (!_cache.TryGetValue(key, out insert))
//                {
//                    ObjectMap map = ObjectMap.Create<TEntity>();

//                    string inserts = map.Properties
//                        .Where(x => x.IsForInsert)
//                        .Select(x => $"{x.ColumnName}")
//                        .Join(", ");

//                    string select = map.Properties
//                        .Where(x => x.IsForInsert)
//                        .Select(x => $"{x.ColumnName}")
//                        .Join(", ");

//                    string outputs = map.OutputColumns
//                        .Select(x => $"inserted.{x.ColumnName}")
//                        .Join(", ");

//                    string output = "";

//                    if (outputs.IsNotEmpty())
//                    {
//                        //  TODO    output = $"output {outputs}";
//                    }

//                    insert = $"insert into {map.FullName} ({inserts}) {output} select {select} from @items";

//                    _cache.Add(key, insert);
//                }

//                return insert;
//            }
//        }

//        #endregion

//        #region Update

//        public static string CreateUpdateOne<TEntity>()
//        {
//            lock (_cache)
//            {
//                string key = "U1:" + typeof(TEntity).FullName;

//                string update = null;

//                if (!_cache.TryGetValue(key, out update))
//                {
//                    ObjectMap map = ObjectMap.Create<TEntity>();

//                    string keys = map.Properties
//                        .Where(x => x.IsPrimaryKey)
//                        .Select(x => $"{x.ColumnName} = @{x.Name}")
//                        .Join(" and ");

//                    string sets = map.Properties
//                        .Where(x => x.IsForUpdate)
//                        .Select(x => $"{x.ColumnName} = @{x.Name}")
//                        .Join(", ");

//                    string outputs = map.OutputColumns
//                        .Where(x => !x.IsPrimaryKey)
//                        .Select(x => $"inserted.{x.ColumnName}")
//                        .Join(", ");

//                    string output = "";

//                    if (outputs.IsNotEmpty())
//                    {
//                        output = $"output {outputs}";
//                    }

//                    update = $"update {map.FullName} set {sets} {output} where {keys}";

//                    _cache.Add(key, update);
//                }

//                return update;
//            }
//        }

//        public static string CreateUpdateMany<TEntity>()
//        {
//            lock (_cache)
//            {
//                string key = "U#:" + typeof(TEntity).FullName;

//                string update = null;

//                if (!_cache.TryGetValue(key, out update))
//                {
//                    ObjectMap map = ObjectMap.Create<TEntity>();

//                    string keys = map.Properties
//                        .Where(x => x.IsPrimaryKey)
//                        .Select(x => $"tgt.{x.ColumnName} = src.{x.ColumnName}")
//                        .Join(" and ");

//                    string sets = map.Properties
//                        .Where(x => x.IsForUpdate)
//                        .Select(x => $"tgt.{x.ColumnName} = src.{x.ColumnName}")
//                        .Join(", ");

//                    string outputs = map.OutputColumns
//                        .Where(x => !x.IsPrimaryKey)
//                        .Select(x => $"inserted.{x.ColumnName}")
//                        .Join(", ");

//                    string output = "";

//                    if (outputs.IsNotEmpty())
//                    {
//                        //  TODO    output = $"output {outputs}";
//                    }

//                    update = $"update tgt set {sets} {output} from {map.FullName} tgt inner join @items src on {keys}";

//                    _cache.Add(key, update);
//                }

//                return update;
//            }
//        }

//        #endregion

//        #region Delete

//        public static string CreateDeleteOne<TEntity>()
//        {
//            lock (_cache)
//            {
//                string key = "D1:" + typeof(TEntity).FullName;

//                string delete = null;

//                if (!_cache.TryGetValue(key, out delete))
//                {
//                    ObjectMap map = ObjectMap.Create<TEntity>();

//                    string keys = map.Properties
//                        .Where(x => x.IsPrimaryKey)
//                        .Select(x => $"{x.ColumnName} = @{x.Name}")
//                        .Join(" and ");

//                    delete = $"delete from {map.FullName} where {keys}";

//                    _cache.Add(key, delete);
//                }

//                return delete;
//            }
//        }

//        public static string CreateDeleteMany<TEntity>()
//        {
//            lock (_cache)
//            {
//                string key = "D#:" + typeof(TEntity).FullName;

//                string delete = null;

//                if (!_cache.TryGetValue(key, out delete))
//                {
//                    ObjectMap map = ObjectMap.Create<TEntity>();

//                    string keys = map.Properties
//                        .Where(x => x.IsPrimaryKey)
//                        .Select(x => $"tgt.{x.ColumnName} = src.{x.ColumnName}")
//                        .Join(" and ");

//                    delete = $"delete tgt from {map.FullName} tgt inner join @items src on {keys}";

//                    _cache.Add(key, delete);
//                }

//                return delete;
//            }
//        }

//        #endregion

//        #region Select

//        public static string CreateSelectOne<TEntity>()
//        {
//            lock (_cache)
//            {
//                string key = "S1:" + typeof(TEntity).FullName;

//                string select = null;

//                if (!_cache.TryGetValue(key, out select))
//                {
//                    ObjectMap map = ObjectMap.Create<TEntity>();

//                    string keys = map.Properties
//                        .Where(x => x.IsPrimaryKey)
//                        .Select(x => $"{x.ColumnName} = @{x.Name}")
//                        .Join(" and ");

//                    select = $"select * from {map.FullName} where {keys}";

//                    _cache.Add(key, select);
//                }

//                return select;
//            }
//        }

//        public static string CreateSelectMany<TEntity>()
//        {
//            lock (_cache)
//            {
//                string key = "S#:" + typeof(TEntity).FullName;

//                string select = null;

//                if (!_cache.TryGetValue(key, out select))
//                {
//                    ObjectMap map = ObjectMap.Create<TEntity>();

//                    select = $"select * from {map.FullName}";

//                    _cache.Add(key, select);
//                }

//                return select;
//            }
//        }

//        #endregion
//    }
//}
