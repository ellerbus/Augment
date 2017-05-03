using System.Collections.Generic;
using System.Data;
using System.Linq;
using Augment.SqlServer.Mapping;
using Dapper;

namespace Augment.SqlServer.Data
{
    static class DapperCrudExtensions
    {
        public static void MergeOne<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateMergeOne<TEntity>();

            ExecuteAndMapOutput(conn, sql, entity);
        }

        public static void MergeMany<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateMergeMany<TEntity>();

            conn.Execute(sql, entity);
        }

        public static void InsertOne<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateInsertOne<TEntity>();

            ExecuteAndMapOutput(conn, sql, entity);
        }

        public static void InsertMany<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateInsertMany<TEntity>();

            conn.Execute(sql, entity);
        }

        public static void UpdateOne<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateUpdateOne<TEntity>();

            ExecuteAndMapOutput(conn, sql, entity);
        }

        public static void UpdateMany<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateUpdateMany<TEntity>();

            conn.Execute(sql, entity);
        }

        public static void DeleteOne<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateDeleteOne<TEntity>();

            conn.Execute(sql, entity);
        }

        public static void DeleteMany<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateDeleteMany<TEntity>();

            conn.Execute(sql, entity);
        }

        public static TEntity SelectOne<TEntity>(this IDbConnection conn, TEntity entity)
        {
            string sql = SqlStatementBuilder.CreateSelectOne<TEntity>();

            TEntity results = conn.Query<TEntity>(sql, entity).FirstOrDefault();

            return results;
        }

        public static IList<TEntity> SelectMany<TEntity>(this IDbConnection conn)
        {
            string sql = SqlStatementBuilder.CreateSelectMany<TEntity>();

            IList<TEntity> results = conn.Query<TEntity>(sql).ToList();

            return results;
        }

        private static void ExecuteAndMapOutput<TEntity>(IDbConnection conn, string sql, TEntity entity)
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
