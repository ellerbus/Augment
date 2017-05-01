using System.Collections.Generic;
using System.Linq;
using Augment.SqlServer.Mapping;

namespace Augment.SqlServer.Data
{
    public static class SqlStatementBuilder
    {
        #region Members

        private static IDictionary<string, string> _cache = new Dictionary<string, string>();

        #endregion

        #region Merge

        public static string CreateMerge<TEntity>()
        {
            lock (_cache)
            {
                string key = "M:" + typeof(TEntity).FullName;

                string merge = null;

                if (!_cache.TryGetValue(key, out merge))
                {
                    TableMap map = TableMap.Create<TEntity>();

                    string keys = map.Columns
                        .Where(x => x.IsPrimaryKey)
                        .Select(x => $"tgt.{x.ColumnName} = src.{x.ColumnName}")
                        .Join(", ");

                    string selects = map.Columns
                        .Select(x => $"@{x.Name} {x.ColumnName}")
                        .Join(", ");

                    string sets = map.Columns
                        .Where(x => x.IsForUpdate)
                        .Select(x => $"tgt.{x.ColumnName} = src.{x.ColumnName}")
                        .Join(", ");

                    string inserts = map.Columns
                        .Where(x => x.IsForInsert)
                        .Select(x => $"{x.ColumnName}")
                        .Join(", ");

                    string values = map.Columns
                        .Where(x => x.IsForInsert)
                        .Select(x => $"src.{x.ColumnName}")
                        .Join(", ");

                    string outputs = map.OutputColumns
                        .Select(x => $"inserted.{x.ColumnName}")
                        .Join(", ");

                    string table = $"merge {map.FullName} as tgt";

                    string select = $"using (select {selects}) as src on ({keys})";

                    string update = $"when matched then update set {sets}";

                    string insert = $"when not matched by target then insert ({inserts}) values ({values})";

                    string output = "";

                    if (map.OutputColumns.Count > 0)
                    {
                        output = $"output {outputs}";
                    }

                    merge = $"{table} {select} {update} {insert} {output} ;";

                    _cache.Add(key, merge);
                }

                return merge;
            }
        }

        #endregion

        #region Insert

        public static string CreateInsert<TEntity>()
        {
            lock (_cache)
            {
                string key = "I:" + typeof(TEntity).FullName;

                string insert = null;

                if (!_cache.TryGetValue(key, out insert))
                {
                    TableMap map = TableMap.Create<TEntity>();

                    string inserts = map.Columns
                        .Where(x => x.IsForInsert)
                        .Select(x => $"{x.ColumnName}")
                        .Join(", ");

                    string values = map.Columns
                        .Where(x => x.IsForInsert)
                        .Select(x => $"@{x.Name}")
                        .Join(", ");

                    string outputs = map.OutputColumns
                        .Select(x => $"inserted.{x.ColumnName}")
                        .Join(", ");

                    string output = "";

                    if (map.OutputColumns.Count > 0)
                    {
                        output = $"output {outputs}";
                    }

                    insert = $"insert into {map.FullName} ({inserts}) {output} values ({values})";

                    _cache.Add(key, insert);
                }

                return insert;
            }
        }

        #endregion

        #region Update

        public static string CreateUpdate<TEntity>()
        {
            lock (_cache)
            {
                string key = "U:" + typeof(TEntity).FullName;

                string update = null;

                if (!_cache.TryGetValue(key, out update))
                {
                    TableMap map = TableMap.Create<TEntity>();

                    string keys = map.Columns
                        .Where(x => x.IsPrimaryKey)
                        .Select(x => $"{x.ColumnName} = @{x.Name}")
                        .Join(", ");

                    string sets = map.Columns
                        .Where(x => x.IsForUpdate)
                        .Select(x => $"{x.ColumnName} = @{x.Name}")
                        .Join(", ");

                    string outputs = map.OutputColumns
                        .Select(x => $"inserted.{x.ColumnName}")
                        .Join(", ");

                    string output = "";

                    if (map.OutputColumns.Count > 0)
                    {
                        output = $"output {outputs}";
                    }

                    update = $"update {map.FullName} set {sets} {output} where {keys}";

                    _cache.Add(key, update);
                }

                return update;
            }
        }

        #endregion

        #region Delete

        public static string CreateDelete<TEntity>()
        {
            lock (_cache)
            {
                string key = "D:" + typeof(TEntity).FullName;

                string delete = null;

                if (!_cache.TryGetValue(key, out delete))
                {
                    TableMap map = TableMap.Create<TEntity>();

                    string keys = map.Columns
                        .Where(x => x.IsPrimaryKey)
                        .Select(x => $"{x.ColumnName} = @{x.Name}")
                        .Join(", ");

                    delete = $"delete from {map.FullName} where {keys}";

                    _cache.Add(key, delete);
                }

                return delete;
            }
        }

        #endregion

        #region Select

        public static string CreateSelectOne<TEntity>()
        {
            lock (_cache)
            {
                string key = "S1:" + typeof(TEntity).FullName;

                string select = null;

                if (!_cache.TryGetValue(key, out select))
                {
                    TableMap map = TableMap.Create<TEntity>();

                    string keys = map.Columns
                        .Where(x => x.IsPrimaryKey)
                        .Select(x => $"tgt.{x.ColumnName} = src.{x.ColumnName}")
                        .Join(", ");

                    select = $"select * from {map.FullName} where {keys}";

                    _cache.Add(key, select);
                }

                return select;
            }
        }

        public static string CreateSelectAll<TEntity>()
        {
            lock (_cache)
            {
                string key = "S*:" + typeof(TEntity).FullName;

                string select = null;

                if (!_cache.TryGetValue(key, out select))
                {
                    TableMap map = TableMap.Create<TEntity>();

                    select = $"select * from {map.FullName}";

                    _cache.Add(key, select);
                }

                return select;
            }
        }

        #endregion
    }
}
