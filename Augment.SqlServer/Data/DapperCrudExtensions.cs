using System.Collections.Generic;
using System.Data;
using System.Linq;
using Augment.SqlServer.Mapping;
using Dapper;

namespace Augment.SqlServer.Data
{
    static class DapperCrudExtensions
    {
        public static void Merge<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateMerge<TEntity>();

            ExecuteAndMapOutput(conn, entity, sql);
        }

        public static void Insert<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateInsert<TEntity>();

            ExecuteAndMapOutput(conn, entity, sql);
        }

        public static void Update<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateUpdate<TEntity>();

            ExecuteAndMapOutput(conn, entity, sql);
        }

        public static void Delete<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateDelete<TEntity>();

            conn.Execute(sql, entity);
        }

        public static TEntity SelectByPrimaryKey<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateSelectOne<TEntity>();

            TEntity results = conn.Query<TEntity>(sql, entity).FirstOrDefault();

            return results;
        }

        public static IList<TEntity> SelectAll<TEntity>(this IDbConnection conn)
        {
            string sql = SqlStatementBuilder.CreateSelectAll<TEntity>();

            IList<TEntity> results = conn.Query<TEntity>(sql).ToList();

            return results;
        }

        private static void ExecuteAndMapOutput<TEntity>(IDbConnection conn, TEntity entity, string sql)
        {
            TableMap map = TableMap.Create<TEntity>();

            if (map.OutputColumns.Count > 0)
            {
                TEntity results = conn.Query<TEntity>(sql, entity).First();

                foreach (ColumnMap col in map.OutputColumns)
                {
                    object value = col.Property.GetValue(results);

                    col.Property.SetValue(entity, value);
                }
            }
            else
            {
                conn.Execute(sql, entity);
            }
        }
    }
}
