using System;
using System.Text;

namespace Augment.SqlServer.Mapping
{
    public static class SqlBuilder
    {
        #region Delete

        public static string CreateDelete<T>()
        {
            return CreateDelete(typeof(T));
        }

        public static string CreateDelete(Type type)
        {
            TableMap table = TableMap.Create(type);

            StringBuilder sql = new StringBuilder($"delete from {table.FullName}");

            AppendWhere(sql, table);

            return sql.ToString();
        }

        private static void AppendWhere(StringBuilder sql, TableMap table)
        {
            bool delim = false;

            foreach (ColumnMap map in table.Columns)
            {
                if (map.IsPrimaryKey)
                {
                    sql.AppendIf(delim, " and ")
                        .Append($"{map.ColumnName} = @{map.Name}");
                }
            }
        }

        #endregion




        //#region Insert
        ///// <summary>
        ///// 指定された型情報から対象となるテーブルにレコードを挿入するクエリを生成します。
        ///// </summary>
        ///// <typeparam name="T">テーブルの型</typeparam>
        ///// <param name="targetDatabase">対象データベース</param>
        ///// <param name="useSequence">シーケンスを利用するかどうか</param>
        ///// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        ///// <returns>生成されたSQL</returns>
        //public static string CreateInsert<T>(DbKind targetDatabase, bool useSequence = true, bool setIdentity = false)
        //    => This.CreateInsert(targetDatabase, typeof(T), useSequence, setIdentity);


        ///// <summary>
        ///// 指定された型情報から対象となるテーブルにレコードを挿入するクエリを生成します。
        ///// </summary>
        ///// <param name="targetDatabase">対象データベース</param>
        ///// <param name="type">テーブルの型</param>
        ///// <param name="useSequence">シーケンスを利用するかどうか</param>
        ///// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        ///// <returns>生成されたSQL</returns>
        //public static string CreateInsert(DbKind targetDatabase, Type type, bool useSequence = true, bool setIdentity = false)
        //{
        //    if (type == null)
        //        throw new ArgumentNullException(nameof(type));

        //    var prefix = targetDatabase.GetBindParameterPrefix();
        //    var table = TableMap.Create(type);
        //    var columns = table.Columns.Where(x => setIdentity ? true : !x.IsAutoIncrement);
        //    var values = columns.Select(x =>
        //    {
        //        if (useSequence)
        //            if (x.Sequence != null)
        //                switch (targetDatabase)
        //                {
        //                    case DbKind.SqlServer: return $"next value for {x.Sequence.FullName}";
        //                    case DbKind.Oracle: return $"{x.Sequence.FullName}.nextval";
        //                    case DbKind.PostgreSql: return $"nextval('{x.Sequence.FullName}')";
        //                }
        //        return $"{prefix}{x.PropertyName}";
        //    })
        //                .Select(x => "    " + x);
        //    var columnNames = columns.Select(x => "    " + x.ColumnName);
        //    var builder = new StringBuilder();
        //    builder.AppendLine($"insert into {table.FullName}");
        //    builder.AppendLine("(");
        //    builder.AppendLine(string.Join($",{Environment.NewLine}", columnNames));
        //    builder.AppendLine(")");
        //    builder.AppendLine("values");
        //    builder.AppendLine("(");
        //    builder.AppendLine(string.Join($",{Environment.NewLine}", values));
        //    builder.Append(")");
        //    return builder.ToString();
        //}
        //#endregion


        //#region Update
        ///// <summary>
        ///// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        ///// </summary>
        ///// <typeparam name="T">テーブルの型</typeparam>
        ///// <param name="targetDatabase">対象データベース</param>
        ///// <param name="properties">抽出する列にマッピングされるプロパティのコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        ///// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        ///// <returns>生成されたSQL</returns>
        //public static string CreateUpdate<T>(DbKind targetDatabase, Expression<Func<T, object>> properties = null, bool setIdentity = false)
        //{
        //    var propertyNames = properties == null
        //                        ? null
        //                        : ExpressionHelper.GetMemberNames(properties);
        //    return This.CreateUpdate(targetDatabase, typeof(T), propertyNames, setIdentity);
        //}


        ///// <summary>
        ///// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        ///// </summary>
        ///// <param name="targetDatabase">対象データベース</param>
        ///// <param name="type">テーブルの型</param>
        ///// <param name="propertyNames">プロパティ名のコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        ///// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        ///// <returns>生成されたSQL</returns>
        //public static string CreateUpdate(DbKind targetDatabase, Type type, IEnumerable<string> propertyNames = null, bool setIdentity = false)
        //{
        //    if (type == null) throw new ArgumentNullException(nameof(type));
        //    if (propertyNames == null) propertyNames = Enumerable.Empty<string>();

        //    var prefix = targetDatabase.GetBindParameterPrefix();
        //    var table = TableMap.Create(type);
        //    var columns = table.Columns.Where(x => setIdentity ? true : !x.IsAutoIncrement);
        //    if (propertyNames.Any())
        //        columns = columns.Join(propertyNames, x => x.PropertyName, y => y, (x, y) => x);
        //    var setters = columns.Select(x => $"    {x.ColumnName} = {prefix}{x.PropertyName}");
        //    var builder = new StringBuilder();
        //    builder.AppendLine($"update {table.FullName}");
        //    builder.AppendLine("set");
        //    builder.Append(string.Join($",{Environment.NewLine}", setters));
        //    return builder.ToString();
        //}
        //#endregion


        //#region Delete
        ///// <summary>
        ///// 指定された型情報から対象となるテーブルのすべてのレコードを削除するクエリを生成します。
        ///// </summary>
        ///// <typeparam name="T">テーブルの型</typeparam>
        ///// <returns>生成されたSQL</returns>
        //public static string CreateDelete<T>() => This.CreateDelete(typeof(T));


        ///// <summary>
        ///// 指定された型情報から対象となるテーブルのすべてのレコードを削除するクエリを生成します。
        ///// </summary>
        ///// <param name="type">テーブルの型</param>
        ///// <returns>生成されたSQL</returns>
        //public static string CreateDelete(Type type)
        //{
        //    if (type == null)
        //        throw new ArgumentNullException(nameof(type));

        //}
        //#endregion
    }
}
