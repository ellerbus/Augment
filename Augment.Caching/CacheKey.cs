using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augment.Caching
{
    class CacheKey
    {
        #region Members

        private List<string> _keys = new List<string>();

        #endregion

        #region Constructors

        public CacheKey(Type type)
        {
            Type baseType = GetBaseType(type);

            if (type == baseType)
            {
                Add(type.FullName);
            }
            else
            {
                Add(baseType.FullName);
                Add("Enumerable");
            }
        }

        #endregion

        #region Methods

        private Type GetBaseType(Type type)
        {
            if (!type.IsValueType && IsImplementationOf(type, typeof(IEnumerable)))
            {
                if (type.IsGenericType)
                {
                    Type[] args = type.GetGenericArguments();

                    if (args.All(x => x.IsValueType))
                    {
                        return args[0];
                    }

                    Type tg = args.First(x => !x.IsValueType);

                    return tg;
                }

                if (type.BaseType.IsGenericType)
                {
                    Type tg = type.BaseType.GetGenericArguments().First(x => !x.IsValueType);

                    return tg;
                }
            }

            return type;
        }

        private static bool IsImplementationOf(Type baseType, Type interfaceType)
        {
            return baseType.GetInterfaces().Any(interfaceType.Equals);
        }

        public void Add(params object[] cacheKeys)
        {
            if (cacheKeys != null)
            {
                if (cacheKeys.Length == 1)
                {
                    _keys.Add(cacheKeys[0].ToString());
                }
                else
                {
                    bool delim = false;

                    StringBuilder key = new StringBuilder();

                    foreach (object o in cacheKeys)
                    {
                        if (delim) key.Append(",");

                        key.Append(o.ToString());

                        delim = true;
                    }

                    _keys.Add(key.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CreateKey()
        {
            return string.Join(";", _keys) + ";";
        }

        #endregion
    }
}
