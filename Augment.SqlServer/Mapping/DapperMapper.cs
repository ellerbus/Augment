using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Augment.SqlServer.Mapping
{
    public static class TypeMapper
    {
        public static void Initialize(Assembly assembly, string namespacePath)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && type.Namespace.IsSameAs(namespacePath))
                {

                }
            }
        }

        public static void Initialize(Type type)
        {
            TableMap tableMap = TableMap.Create(type);

            SqlMapper.ITypeMap typeMap = SqlMapper.GetTypeMap(type);

            CustomTypeMap mapper = new CustomTypeMap(type, typeMap, tableMap);

            SqlMapper.SetTypeMap(type, mapper);
        }
    }

    class CustomTypeMap : SqlMapper.ITypeMap
    {
        #region Members

        private Type _type;

        private readonly SqlMapper.ITypeMap _originalMap;

        private TableMap _tableMap;

        #endregion

        #region Constructors

        public CustomTypeMap(Type type, SqlMapper.ITypeMap originalMap, TableMap tableMap)
        {
            _type = type;

            _originalMap = originalMap;

            _tableMap = tableMap;
        }

        #endregion

        #region Methods

        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            IEnumerable<ConstructorInfo> constructors = _type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .OrderByDescending(c => c.GetParameters().Length)
                .ThenBy(c => c.IsPublic ? 0 : (c.IsPrivate ? 2 : 1));

            foreach (ConstructorInfo ctor in constructors)
            {
                ParameterInfo[] ctorParameters = ctor.GetParameters();

                if (ctorParameters.Length == 0)
                {
                    return ctor;
                }

                if (ctorParameters.Length != types.Length)
                {
                    continue;
                }

                int i = 0;

                for (; i < ctorParameters.Length; i++)
                {
                    string parmName = ctorParameters[i].Name;

                    if (parmName.IsNotSameAs(names[i].Replace("_", "")))
                    {
                        break;
                    }

                    //if (types[i] == typeof(byte[]) && ctorParameters[i].ParameterType.FullName == SqlMapper.LinqBinary)
                    //    continue;
                    //var unboxedType = Nullable.GetUnderlyingType(ctorParameters[i].ParameterType) ?? ctorParameters[i].ParameterType;
                    //if ((unboxedType != types[i] && !SqlMapper.HasTypeHandler(unboxedType))
                    //    && !(unboxedType.IsEnum() && Enum.GetUnderlyingType(unboxedType) == types[i])
                    //    && !(unboxedType == typeof(char) && types[i] == typeof(string))
                    //    && !(unboxedType.IsEnum() && types[i] == typeof(string)))
                    //{
                    //    break;
                    //}
                }

                if (i == ctorParameters.Length)
                {
                    return ctor;
                }
            }

            return _originalMap.FindConstructor(names, types);
        }

        public ConstructorInfo FindExplicitConstructor()
        {
            return _originalMap.FindExplicitConstructor();
        }

        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            ColumnMap map = _tableMap.Columns.FirstOrDefault(x => x.ColumnName.IsSameAs(columnName));

            if (map != null)
            {
                ParameterInfo parm = constructor.GetParameters().First(x => x.Name.IsSameAs(map.Name));

                return new CustomMemberMap(map, parm);
            }

            return _originalMap.GetConstructorParameter(constructor, columnName);
        }

        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            ColumnMap map = _tableMap.Columns.FirstOrDefault(x => x.ColumnName.IsSameAs(columnName));

            if (map != null)
            {
                return new CustomMemberMap(map);
            }

            return null;
        }

        #endregion
    }

    class CustomMemberMap : SqlMapper.IMemberMap
    {
        #region Members

        private ColumnMap _map;

        #endregion

        #region Constructors

        public CustomMemberMap(ColumnMap map)
        {
            _map = map;
        }

        public CustomMemberMap(ColumnMap map, ParameterInfo parm)
            : this(map)
        {
            Parameter = parm;
        }

        #endregion

        #region Properties

        public string ColumnName { get { return _map.ColumnName; } }

        public Type MemberType
        {
            get
            {
                if (Parameter == null)
                {
                    return Parameter.ParameterType;
                }

                return _map.Type;
            }
        }

        public FieldInfo Field { get { return null; } }

        public ParameterInfo Parameter { get; private set; }

        public PropertyInfo Property { get { return _map.Property; } }

        #endregion
    }
}
