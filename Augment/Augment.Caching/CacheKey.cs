using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Augment.Caching
{
    class CacheKey
    {
        #region Static Members

        private static object _lock = new object();

        private static Dictionary<Type, TypeMap> _baseTypeMaps = new Dictionary<Type, TypeMap>();

        private const string _delimiter = ";";

        private static readonly string _enumerableKey = typeof(IEnumerable<>).Name;

        class TypeMap
        {
            public Type CacheType;
            public Type BaseType;
            public bool IsEnumerable;
        }

        #endregion

        #region Members

        private Type _type;

        private Type _baseType;

        private bool _isEnumerable;

        private List<string> _keys = new List<string>();

        #endregion

        #region Constructors

        public CacheKey(Type type)
        {
            _type = type;

            lock (_lock)
            {
                TypeMap bt = null;

                if (!_baseTypeMaps.TryGetValue(type, out bt))
                {
                    bt = GetBaseType(type);

                    _baseTypeMaps.Add(type, bt);
                }

                _baseType = bt.BaseType;

                _isEnumerable = bt.IsEnumerable;
            }
        }

        #endregion

        #region Methods

        private static TypeMap GetBaseType(Type type)
        {
            TypeMap bt = new TypeMap
            {
                CacheType = type,
                BaseType = type
            };

            foreach (Type t in type.GetInterfaces().Where(x => x.IsGenericType))
            {
                if (t.Name == typeof(IEnumerable<>).Name)
                {
                    bt.IsEnumerable = true;

                    Type[] types = t.GetGenericArguments();

                    if (types.Length == 1 && types[0].Name == typeof(KeyValuePair<,>).Name)
                    {
                        types = types[0].GetGenericArguments();
                    }

                    bt.BaseType = types.FirstOrDefault(x => !x.IsValueType) ?? type;

                    break;
                }
            }

            return bt;
        }

        public void Add(params object[] cacheKeys)
        {
            if (cacheKeys != null)
            {
                _keys.AddRange(cacheKeys.Select(x => x.ToString()));
            }
        }

        /// <summary>
        /// Gets the cache-key Namespace.Object;by,by,by;+
        /// </summary>
        public string CreateKey()
        {
            StringBuilder key = CreateKey(false);

            if (_isEnumerable)
            {
                key.Append(_enumerableKey);
            }

            System.Diagnostics.Debug.WriteLine(key.ToString());

            return key.ToString();
        }

        /// <summary>
        /// Gets the cache-key Namespace.Object;by,*,by;+
        /// </summary>
        public string CreateRemoveAllKeyPattern()
        {
            StringBuilder key = CreateKey(true);

            if (_isEnumerable)
            {
                key.Append(_enumerableKey);
            }
            else
            {
                //  this is the base type and we want to
                //  remove all, so assuming anything
                //  of type BaseType
                key.Append(".*");
            }

            return key.Insert(0, "^").Append("$").ToString();
        }

        private StringBuilder CreateKey(bool forRegexPattern)
        {
            string baseKey = _baseType.FullName;

            if (forRegexPattern)
            {
                baseKey = CreateRegexPattern(baseKey);
            }

            StringBuilder sb = new StringBuilder(baseKey)
                .Append(_delimiter)
                .Append(GetFilterKey(forRegexPattern))
                .Append(_delimiter);

            return sb;
        }

        private string GetFilterKey(bool forRegexPattern)
        {
            string key = string.Join(",", _keys);

            if (forRegexPattern)
            {
                key = CreateRegexPattern(key);
            }

            return key;
        }

        private string CreateRegexPattern(string s)
        {
            string pattern = s;

            int pos = pattern.IndexOf("**");

            while (pos >= 0)
            {
                pattern = pattern.Replace("**", "*");

                pos = pattern.IndexOf("**");
            }

            //  escape all regex characters except *
            pattern = Regex.Replace(pattern, @"[\.\$\^\{\[\(\|\)\]\}\+\\\?]", m => @"\" + m.Value, RegexOptions.Compiled);

            //  replace * with .*
            pattern = Regex.Replace(pattern, @"\*", m => "." + m.Value, RegexOptions.Compiled);

            return pattern;
        }

        #endregion
    }
}
