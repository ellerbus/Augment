using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Augment.Caching
{

    class CacheKey
    {
        #region Members

        private const string _delimiter = ";";

        private const string _enumerableKey = "+";

        private Type _type;

        private Type _baseType;

        private bool _isEnumerable;

        private List<string> _keys = new List<string>();

        #endregion

        #region Constructors

        public CacheKey(Type type)
        {
            _type = type;

            _baseType = GetBaseType(type);
        }

        #endregion

        #region Methods

        private Type GetBaseType(Type type)
        {
            if (!type.IsValueType && type.IsGenericType && IsImplementationOf(type, typeof(IEnumerable)))
            {
                _isEnumerable = true;

                Type tg = type.GetGenericArguments().First(x => !x.IsValueType);

                return tg;
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

            return key.ToString();
        }
        /// <summary>
        /// Gets the cache-key Namespace.Object;by,*,by;+
        /// </summary>
        public string CreateRemoveAllKeyPattern()
        {
            StringBuilder key = CreateKey(true);

            key.Append(@"\").Append(_enumerableKey);

            if (!_isEnumerable)
            {
                //  this is the base type and we want to
                //  remove all, so assuming anything
                //  of type BaseType
                key.Append("?");
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
