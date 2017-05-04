//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Dapper;

//namespace Augment.SqlServer.Mapping
//{
//    static class TypeMapper
//    {
//        public static void Initialize(Type type, ObjectMap tableMap)
//        {
//            SqlMapper.ITypeMap typeMap = SqlMapper.GetTypeMap(type);

//            CustomTypeMap mapper = new CustomTypeMap(type, typeMap, tableMap);

//            SqlMapper.SetTypeMap(type, mapper);
//        }
//    }

//    class CustomTypeMap : SqlMapper.ITypeMap
//    {
//        #region Members

//        private Type _type;

//        private readonly SqlMapper.ITypeMap _originalMap;

//        private ObjectMap _tableMap;

//        #endregion

//        #region Constructors

//        public CustomTypeMap(Type type, SqlMapper.ITypeMap originalMap, ObjectMap tableMap)
//        {
//            _type = type;

//            _originalMap = originalMap;

//            _tableMap = tableMap;
//        }

//        #endregion

//        #region Methods

//        public ConstructorInfo FindConstructor(string[] names, Type[] types)
//        {
//            IEnumerable<ConstructorInfo> constructors = _type
//                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
//                .OrderByDescending(c => c.GetParameters().Length)
//                .ThenBy(c => c.IsPublic ? 0 : (c.IsPrivate ? 2 : 1));

//            foreach (ConstructorInfo ctor in constructors)
//            {
//                ParameterInfo[] parameters = ctor.GetParameters();

//                if (parameters.Length == 0)
//                {
//                    //  last one so use it (order-by)
//                    return ctor;
//                }

//                if (parameters.All(x => x.ParameterType.IsPotentialPrimitive()))
//                {
//                    //  all primitives so use it
//                    return ctor;
//                }
//            }

//            return _originalMap.FindConstructor(names, types);
//        }

//        public ConstructorInfo FindExplicitConstructor()
//        {
//            return null;
//        }

//        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
//        {
//            PropertyMap map = _tableMap.Properties.FirstOrDefault(x => x.ColumnName.IsSameAs(columnName));

//            if (map != null)
//            {
//                ParameterInfo parm = constructor.GetParameters().FirstOrDefault(x => x.Name.IsSameAs(map.Name));

//                if (parm != null)
//                {
//                    return new CustomMemberMap(map, parm);
//                }
//            }

//            return null;
//        }

//        public SqlMapper.IMemberMap GetMember(string columnName)
//        {
//            PropertyMap map = _tableMap.Properties.FirstOrDefault(x => x.ColumnName.IsSameAs(columnName));

//            if (map != null)
//            {
//                return new CustomMemberMap(map);
//            }

//            return null;
//        }

//        #endregion
//    }

//    class CustomMemberMap : SqlMapper.IMemberMap
//    {
//        #region Members

//        private PropertyMap _map;

//        #endregion

//        #region Constructors

//        public CustomMemberMap(PropertyMap map)
//        {
//            _map = map;
//        }

//        public CustomMemberMap(PropertyMap map, ParameterInfo parm)
//            : this(map)
//        {
//            Parameter = parm;
//        }

//        #endregion

//        #region Properties

//        public string ColumnName { get { return _map.ColumnName; } }

//        public Type MemberType
//        {
//            get { return _map.Type; }
//        }

//        public FieldInfo Field { get { return null; } }

//        public ParameterInfo Parameter { get; private set; }

//        public PropertyInfo Property { get { return _map.Property; } }

//        #endregion
//    }
//}
