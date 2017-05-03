using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Augment.SqlServer.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{Name,nq} -> {TableName,nq}")]
    public sealed class TableMap
    {
        #region Members

        /// <summary>
        /// 
        /// </summary>
        private static readonly IDictionary<Type, TableMap> _cache = new Dictionary<Type, TableMap>();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private TableMap(Type type)
        {
            TableAttribute table = type.GetCustomAttribute<TableAttribute>(false);

            Schema = table?.Schema ?? null;
            TableName = table?.Name ?? type.Name;
            Type = type;

            Type columnAttribute = typeof(ColumnAttribute);

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            Columns = type.GetProperties(flags)
                .Where(x => x.CustomAttributes.Any(y => y.AttributeType == columnAttribute))
                .Select(x => ColumnMap.Create(x))
                .ToArray();

            OutputColumns = Columns.Where(x => x.IsForOutput).ToArray();

            TypeMapper.Initialize(type, this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TableMap Create<T>()
        {
            return Create(typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TableMap Create(Type type)
        {
            TableMap map = null;

            lock (_cache)
            {
                if (!_cache.TryGetValue(type, out map))
                {
                    map = new TableMap(type);

                    _cache.Add(type, map);
                }
            }

            return map;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Type Name
        /// </summary>
        public string Name { get { return Type.Name; } }

        /// <summary>
        /// 
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Schema { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<ColumnMap> Columns { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<ColumnMap> OutputColumns { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string FullName { get { return Schema.IsNullOrEmpty() ? TableName : $"{Schema}.{TableName}"; } }

        #endregion
    }
}