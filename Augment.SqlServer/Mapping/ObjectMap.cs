using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Augment.SqlServer.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{Name,nq} -> {FullName,nq}")]
    sealed class ObjectMap
    {
        #region Members

        /// <summary>
        /// 
        /// </summary>
        private static readonly IDictionary<Type, ObjectMap> _cache = new Dictionary<Type, ObjectMap>();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ObjectMap(Type type)
        {
            SchemaName = "dbo";
            TableName = type.Name;
            Type = type;

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            Properties = type.GetProperties(flags)
                .Where(x => x.CanRead && x.CanWrite)
                .Select(x => new PropertyMap(x))
                .ToDictionary(x => x.NormalizedName);

            Constructor = new ObjectConstructor(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ObjectMap GetDefaultMap<T>()
        {
            Type type = typeof(T);

            ObjectMap map = null;

            lock (_cache)
            {
                if (!_cache.TryGetValue(type, out map))
                {
                    map = new ObjectMap(type);

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
        public string SchemaName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<string, PropertyMap> Properties { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ObjectConstructor Constructor { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string FullName { get { return SchemaName.IsNullOrEmpty() ? TableName : $"{SchemaName}.{TableName}"; } }

        #endregion
    }
}