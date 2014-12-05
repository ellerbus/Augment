using System;
using System.Collections.Generic;
using EnsureThat;

namespace Augment
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        #region Misc

        ///// <summary>
        ///// Shortcut for string.IsNullOrEmpty()
        ///// </summary>
        ///// <param name="s"></param>
        ///// <returns></returns>
        //public static bool IsNullOrEmpty(this string s)
        //{
        //    return string.IsNullOrEmpty(s);
        //}

        ///// <summary>
        ///// Shortcut for !string.IsNullOrEmpty()
        ///// </summary>
        ///// <param name="s"></param>
        ///// <returns></returns>
        //public static bool IsNotEmpty(this string s)
        //{
        //    return !string.IsNullOrEmpty(s);
        //}

        /// <summary>
        /// Shortcut for string.IsNullOrWhiteSpace()
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// Shortcut for string.Compare case insensitivie, but will NOT test if empty or null
        /// </summary>
        /// <param name="s"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsSameAs(this string s, string other)
        {
            Ensure.That(s).IsNotNull();
            Ensure.That(other).IsNotNull();

            return string.Compare(s, other, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// Instead of string.Format("Hello {0}", "Joe") "Hello {0}".FormatArgs("Joe") looks cleaner
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public static string FormatArgs(this string s, object arg0)
        {
            Ensure.That(s).IsNotNull();

            return string.Format(s, arg0);
        }

        /// <summary>
        /// Instead of string.Format("Hello {0}", "Joe") "Hello {0}".FormatArgs("Joe") looks cleaner
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static string FormatArgs(this string s, object arg0, object arg1)
        {
            Ensure.That(s).IsNotNull();

            return string.Format(s, arg0, arg1);
        }

        /// <summary>
        /// Instead of string.Format("Hello {0}", "Joe") "Hello {0}".FormatArgs("Joe") looks cleaner
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static string FormatArgs(this string s, object arg0, object arg1, object arg2)
        {
            Ensure.That(s).IsNotNull();

            return string.Format(s, arg0, arg1, arg2);
        }

        /// <summary>
        /// Instead of string.Format("Hello {0}", "Joe") "Hello {0}".FormatArgs("Joe") looks cleaner
        /// </summary>
        /// <param name="s"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatArgs(this string s, params object[] args)
        {
            Ensure.That(s).IsNotNull();

            return string.Format(s, args);
        }

        /// <summary>
        /// Asserts that a string is not NULL but at least empty
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string AssertNotNull(this string s, string defaultValue = "")
        {
            if (s == null)
            {
                return defaultValue;
            }

            return s;
        }

        /// <summary>
        /// Left 'x' characters
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(this string value, int length)
        {
            Ensure.That(value).IsNotNull();

            if (value.Length < length)
            {
                return value;
            }

            return value.Substring(0, length);
        }

        /// <summary>
        /// Gets the left of a specified string, or the string if NOT found
        /// </summary>
        /// <param name="value"></param>
        /// <param name="leftOf"></param>
        /// <returns></returns>
        public static string GetLeftOf(this string value, string leftOf)
        {
            Ensure.That(value).IsNotNull();

            int index = value.IndexOf(leftOf);

            if (index > -1)
            {
                return value.Substring(0, index);
            }

            return value;
        }

        /// <summary>
        /// Right 'x' characters of string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string value, int length)
        {
            Ensure.That(value).IsNotNull();

            if (value.Length < length)
            {
                return value;
            }

            return value.Substring(value.Length - length);
        }

        /// <summary>
        /// Gets the right of a specified string, or the string if NOT found
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rightOf"></param>
        /// <returns></returns>
        public static string GetRightOf(this string value, string rightOf)
        {
            Ensure.That(value).IsNotNull();

            int index = value.IndexOf(rightOf);

            if (index > -1)
            {
                return value.Substring(index + rightOf.Length);
            }

            return value;
        }

        /// <summary>
        /// (new [] {"A", "B"}).Join(", ")
        /// </summary>
        /// <param name="values"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }

        #endregion

        #region Enum Translations

        private static Dictionary<string, Dictionary<string, object>> _stringToEnum = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// Converts a string to an enum
        /// </summary>
        /// <typeparam name="T">Desired Enum to translate to</typeparam>
        /// <param name="stringOrDescription">String or System.ComponentModel.Description</param>
        /// <returns>Translated Enum</returns>
        public static T ToEnum<T>(this string stringOrDescription)
        {
            string t = typeof(T).FullName;

            if (!_stringToEnum.ContainsKey(t))
            {
                _stringToEnum.Add(t, new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase));
            }

            if (!_stringToEnum[t].ContainsKey(stringOrDescription))
            {
                _stringToEnum[t].Add(stringOrDescription, EnumExtensions.GetEnumObject(typeof(T), stringOrDescription));
            }

            return (T)_stringToEnum[t][stringOrDescription];
        }

        #endregion
    }
}
