using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Augment.SqlServer.Mapping;

namespace Augment.SqlServer
{
    static class SqlConnectionExtensions
    {
        #region Connection Extensions

        public static void Execute(this SqlConnection conn, string sql)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = GetCommandType(sql);

                cmd.ExecuteNonQuery();
            }
        }

        public static T ExecuteScalar<T>(this SqlConnection conn, string sql)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = GetCommandType(sql);

                object value = cmd.ExecuteScalar();

                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        public static IList<T> Query<T>(this SqlConnection conn, string sql) where T : class
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = GetCommandType(sql);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (typeof(T).IsPotentialMappableClass())
                    {
                        return QueryWithMap<T>(reader, sql).ToList();
                    }
                    else
                    {
                        return QueryScalar<T>(reader, sql).ToList();
                    }
                }
            }
        }

        private static IEnumerable<T> QueryWithMap<T>(SqlDataReader reader, string sql) where T : class
        {
            ObjectMap map = ObjectMap.GetDefaultMap<T>();

            Dictionary<string, string> normalizationMap = GetNormalizedMap(reader);

            while (reader.Read())
            {
                T entity = (T)map.Constructor.Create(normalizationMap, reader);

                yield return entity;
            }
        }

        private static IEnumerable<T> QueryScalar<T>(SqlDataReader reader, string sql) where T : class
        {
            while (reader.Read())
            {
                T entity = (T)reader[0];

                yield return entity;
            }
        }

        private static Dictionary<string, string> GetNormalizedMap(SqlDataReader reader)
        {
            Dictionary<string, string> mapping = new Dictionary<string, string>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string column = reader.GetName(i);

                string normalized = column.ToLower().Replace("_", "");

                mapping.Add(normalized, column);
            }

            return mapping;
        }

        //private static void Execute<TEntity>(SqlConnection conn, string sql, TEntity entity, IEnumerable<ColumnMap> columns)
        //{
        //    using (SqlCommand cmd = conn.CreateCommand() )
        //    {
        //        cmd.CommandText = sql;
        //        cmd.CommandType = GetCommandType(sql);

        //        foreach (ColumnMap col in columns)
        //        {
        //            cmd.Parameters.AddWithValue(col.Name, col.Property.GetValue(entity));
        //        }

        //        cmd.ExecuteNonQuery();
        //    }
        //}

        #endregion

        #region CRUD Operations

        //public static void MergeOne<TEntity>(this SqlConnection conn, TEntity entity)
        //{
        //    string sql = SqlStatementBuilder.CreateMergeOne<TEntity>();

        //    ExecuteAndMapOutput(conn, sql, entity);
        //}

        //public static void MergeMany<TEntity>(this SqlConnection conn, IEnumerable<TEntity> entities)
        //{
        //    string sql = SqlStatementBuilder.CreateMergeMany<TEntity>();

        //    ExecuteMany(conn, sql, entities);
        //}

        //public static void InsertOne<TEntity>(this SqlConnection conn, TEntity entity)
        //{
        //    string sql = SqlStatementBuilder.CreateInsertOne<TEntity>();

        //    ExecuteAndMapOutput(conn, sql, entity);
        //}

        //public static void InsertMany<TEntity>(this SqlConnection conn, IEnumerable<TEntity> entities)
        //{
        //    string sql = SqlStatementBuilder.CreateInsertMany<TEntity>();

        //    ExecuteMany(conn, sql, entities);
        //}

        //public static void UpdateOne<TEntity>(this SqlConnection conn, TEntity entity)
        //{
        //    string sql = SqlStatementBuilder.CreateUpdateOne<TEntity>();

        //    ExecuteAndMapOutput(conn, sql, entity);
        //}

        //public static void UpdateMany<TEntity>(this SqlConnection conn, IEnumerable<TEntity> entities)
        //{
        //    string sql = SqlStatementBuilder.CreateUpdateMany<TEntity>();

        //    ExecuteMany(conn, sql, entities);
        //}

        //public static void DeleteOne<TEntity>(this SqlConnection conn, TEntity entity)
        //{
        //    string sql = SqlStatementBuilder.CreateDeleteOne<TEntity>();

        //    Execute(conn, sql, entity, columns);
        //}

        //public static void DeleteMany<TEntity>(this SqlConnection conn, IEnumerable<TEntity> entities)
        //{
        //    string sql = SqlStatementBuilder.CreateDeleteMany<TEntity>();

        //    ExecuteMany(conn, sql, entities);
        //}

        //public static TEntity SelectOne<TEntity>(this SqlConnection conn, TEntity entity)
        //{
        //    string sql = SqlStatementBuilder.CreateSelectOne<TEntity>();

        //    TEntity results = conn.Query<TEntity>(sql, entity).FirstOrDefault();

        //    return results;
        //}

        //public static IList<TEntity> SelectMany<TEntity>(this SqlConnection conn)
        //{
        //    string sql = SqlStatementBuilder.CreateSelectMany<TEntity>();

        //    IList<TEntity> results = conn.Query<TEntity>(sql).ToList();

        //    return results;
        //}

        #endregion

        #region Helpers

        private static CommandType GetCommandType(string sql)
        {
            if (sql.Contains(" "))
            {
                return CommandType.Text;
            }

            return CommandType.StoredProcedure;
        }

        //private static void ExecuteMany<TEntity>(SqlConnection conn, string sql, IEnumerable<TEntity> entities)
        //{
        //    TableMap map = TableMap.Create<TEntity>();

        //    using (SqlCommand cmd = conn.CreateCommand() )
        //    {
        //        cmd.CommandText = sql;
        //        cmd.CommandType = GetCommandType(sql);

        //        SqlParameter p = cmd.Parameters.Add("items", SqlDbType.Structured);

        //        p.Value = ToDataTable(map, entities);
        //        p.TypeName = $"{map.Schema}.{map.TableName}TableType";

        //        cmd.ExecuteNonQuery();
        //    }
        //}

        //private static DataTable ToDataTable<TEntity>(TableMap map, IEnumerable<TEntity> entities)
        //{
        //    DataTable table = new DataTable();

        //    IList<ColumnMap> columns = map.Columns.Where(x => !x.IsTimestamp && !x.IsCalculated).ToList();

        //    foreach (ColumnMap col in columns)
        //    {
        //        table.Columns.Add(col.ColumnName, col.Property.PropertyType);
        //    }

        //    foreach (TEntity item in entities)
        //    {
        //        DataRow row = table.NewRow();

        //        foreach (ColumnMap col in columns)
        //        {
        //            row[col.ColumnName] = col.Property.GetValue(item);
        //        }

        //        table.Rows.Add(row);
        //    }

        //    return table;
        //}

        //private static void ExecuteAndMapOutput<TEntity>(SqlConnection conn, string sql, TEntity entity, IEnumerable<ColumnMap> columns)
        //{
        //    TableMap map = TableMap.Create<TEntity>();

        //    if (map.OutputColumns.Count > 0)
        //    {
        //        TEntity results = conn.Query<TEntity>(sql, entity).First();

        //        foreach (ColumnMap col in map.OutputColumns)
        //        {
        //            object value = col.Property.GetValue(results);

        //            col.Property.SetValue(entity, value);
        //        }
        //    }
        //    else
        //    {
        //        Execute(conn, sql, cmd =>
        //        {
        //            foreach (ColumnMap col in map.Columns.Where(x => x.IsForDelete))
        //            {
        //                cmd.Parameters.AddWithValue(col.Name, col.Property.GetValue(entity));
        //            }
        //        });
        //    }
        //}

        #endregion
    }
}
