using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Augment
{
    /// <summary>
    /// Handy Enum Extensions
    /// </summary>
    public static class EnumExtensions
    {
        #region Enum Translations

        private static Dictionary<string, Dictionary<Enum, string>> _enumToString = new Dictionary<string, Dictionary<Enum, string>>();

        /// <summary>
        /// Gets the System.ComponentModel.Description attribute if it exists, or
        /// calls ToString if it does not
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum val)
        {
            string t = val.GetType().FullName;

            lock (_enumToString)
            {
                if (!_enumToString.ContainsKey(t))
                {
                    _enumToString.Add(t, new Dictionary<Enum, string>());
                }

                if (!_enumToString[t].ContainsKey(val))
                {
                    _enumToString[t].Add(val, GetDescriptionForEnum(val));
                }
            }

            return _enumToString[t][val];
        }

        /// <summary>
        /// Gets an enum object from a string or System.ComponentModel.Description attribute
        /// </summary>
        /// <param name="t"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        internal static object GetEnumObject(Type t, string description)
        {
            foreach (Enum e in Enum.GetValues(t))
            {
                string nm = e.ToString();

                bool isMatch = nm.Equals(description, StringComparison.InvariantCultureIgnoreCase)
                    || description.Equals(GetDescriptionForEnum(e), StringComparison.InvariantCultureIgnoreCase)
                    ;

                if (isMatch)
                {
                    FieldInfo i = e.GetType().GetField(nm);

                    return i.GetValue(e);
                }
            }

            throw new Exception("{0} not found for {1}".FormatArgs(description, t.Name));
        }

        /// <summary>
        /// Gets the System.ComponentModel.Description attribute if it exists, or
        /// calls ToString if it does not
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static string GetDescriptionForEnum(Enum val)
        {
            Type t = val.GetType();

            return GetDescriptionForEnum(t, val.ToString());
        }

        /// <summary>
        /// Gets the System.ComponentModel.Description attribute if it exists, or
        /// calls ToString if it does not
        /// </summary>
        /// <param name="t"></param>
        /// <param name="sval"></param>
        /// <returns></returns>
        private static string GetDescriptionForEnum(Type t, string sval)
        {
            FieldInfo fi = t.GetField(sval);

            if (fi != null)
            {
                IEnumerable<DescriptionAttribute> attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true)
                    .Select(a => a as DescriptionAttribute)
                    ;

                foreach (DescriptionAttribute da in attrs)
                {
                    return da.Description;
                }
            }

            return sval;
        }

        #endregion
    }
}