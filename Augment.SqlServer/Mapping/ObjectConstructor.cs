using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Augment.SqlServer.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    sealed class ObjectConstructor
    {
        #region Members

        private ObjectMap _map;
        private ConstructorInfo _constructor;
        private Dictionary<string, ParameterMap> _parameters;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ObjectConstructor(ObjectMap map)
        {
            _map = map;

            _constructor = FindConstructor();

            _parameters = GetParameters();
        }

        #endregion

        #region Methods

        public object Create(Dictionary<string, string> normalizationMap, SqlDataReader reader)
        {
            object entity = null;

            if (_parameters.Count == 0)
            {
                entity = _constructor.Invoke(null);

                foreach (var names in normalizationMap)
                {
                    if (_map.Properties.ContainsKey(names.Key))
                    {
                        object value = reader[names.Value];

                        _map.Properties[names.Key].Property.SetValue(entity, value);
                    }
                }
            }
            else
            {
                object[] parms = new object[_parameters.Count];

                foreach (var map in _parameters)
                {
                    if (normalizationMap.ContainsKey(map.Value.NormalizedName))
                    {
                        string column = normalizationMap[map.Value.NormalizedName];

                        object value = reader[column];

                        parms[map.Value.Index] = value;
                    }
                    else
                    {
                        parms[map.Value.Index] = map.Value.Parameter.ParameterType.DefaultValue();
                    }
                }

                entity = _constructor.Invoke(parms);
            }

            return entity;
        }

        private ConstructorInfo FindConstructor()
        {
            IEnumerable<ConstructorInfo> constructors = _map.Type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .OrderByDescending(c => c.GetParameters().Length)
                .ThenBy(c => c.IsPublic ? 0 : (c.IsPrivate ? 2 : 1));

            foreach (ConstructorInfo ctor in constructors)
            {
                ParameterInfo[] parameters = ctor.GetParameters();

                if (parameters.Length == 0)
                {
                    //  last one so use it (order-by) if empty constructor
                    return ctor;
                }

                if (parameters.All(x => x.ParameterType.IsPotentialPrimitive()))
                {
                    //  all primitives so use it
                    return ctor;
                }
            }

            throw new Exception($"Unable to find constructor for: '{_map.FullName}'");
        }

        private Dictionary<string, ParameterMap> GetParameters()
        {
            Dictionary<string, ParameterMap> parms = new Dictionary<string, ParameterMap>();

            ParameterInfo[] parameters = _constructor.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterMap map = new ParameterMap(i, parameters[i]);

                parms.Add(map.NormalizedName, map);
            }

            return parms;
        }

        #endregion
    }
}