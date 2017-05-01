using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using EnsureThat;

namespace Augment.SqlServer.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{Name,nq} -> {ColumnName,nq}")]
    public sealed class ColumnMap
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private ColumnMap() { }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static ColumnMap Create(PropertyInfo pi)
        {
            ColumnMap map = new ColumnMap();

            map.Property = pi;

            map.ColumnName = GetColumnName(pi);
            map.ColumnType = TypeMap.Default[pi.PropertyType];

            map.IsPrimaryKey = Has<KeyAttribute>(pi);
            map.IsNullable = !map.IsPrimaryKey && !HasRequired(pi);
            map.IsIdentity = HasDatabaseGeneratedOption(pi, DatabaseGeneratedOption.Identity);
            map.IsCalculated = HasDatabaseGeneratedOption(pi, DatabaseGeneratedOption.Computed);
            map.IsTimestamp = Has<TimestampAttribute>(pi);

            return map;
        }

        private static string GetColumnName(PropertyInfo info)
        {
            Ensure.That(info, "ParameterInfo").IsNotNull();

            ColumnAttribute attribute = info.GetCustomAttribute<ColumnAttribute>(false);

            return attribute?.Name ?? info.Name;
        }

        private static bool HasRequired(PropertyInfo info)
        {
            Ensure.That(info, "ParameterInfo").IsNotNull();

            if (Has<RequiredAttribute>(info))
            {
                return true;
            }

            TypeCode code = Type.GetTypeCode(info.PropertyType);

            switch (code)
            {
                case TypeCode.String:
                case TypeCode.Object:
                    return false;

                default:
                    return true;
            }
        }

        private static bool Has<TAttribute>(PropertyInfo info) where TAttribute : Attribute
        {
            Ensure.That(info, "ParameterInfo").IsNotNull();

            return Attribute.IsDefined(info, typeof(TAttribute));
        }

        private static bool HasDatabaseGeneratedOption(PropertyInfo info, DatabaseGeneratedOption matches)
        {
            Ensure.That(info, "ParameterInfo").IsNotNull();

            DatabaseGeneratedAttribute attribute = info.GetCustomAttribute<DatabaseGeneratedAttribute>(false);

            DatabaseGeneratedOption option = attribute?.DatabaseGeneratedOption ?? DatabaseGeneratedOption.None;

            return option == matches;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Gets the Property Name
        /// </summary>
        public string Name { get { return Property.Name; } }

        /// <summary>
        /// 
        /// </summary>
        public Type Type { get { return Property.PropertyType; } }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public DbType ColumnType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPrimaryKey { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNullable { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsIdentity { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCalculated { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsTimestamp { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsForInsert { get { return !IsIdentity && !IsCalculated && !IsTimestamp; } }

        /// <summary>
        /// 
        /// </summary>
        public bool IsForUpdate { get { return !IsPrimaryKey && !IsIdentity && !IsCalculated && !IsTimestamp; } }

        /// <summary>
        /// 
        /// </summary>
        public bool IsForDelete { get { return !IsPrimaryKey; } }

        /// <summary>
        /// 
        /// </summary>
        public bool IsForOutput { get { return IsIdentity || IsCalculated || IsTimestamp; } }

        #endregion
    }
}