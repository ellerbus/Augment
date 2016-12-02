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
        /// Shortcut for !IsSameAs
        /// </summary>
        /// <param name="s"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsNotSameAs(this string s, string other)
        {
            return !IsSameAs(s, other);
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
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(this string s, int length)
        {
            Ensure.That(s).IsNotNull();

            if (s.Length < length)
            {
                return s;
            }

            return s.Substring(0, length);
        }

        /// <summary>
        /// Gets the left of a specified string, or the string if NOT found
        /// </summary>
        /// <param name="s"></param>
        /// <param name="leftOf"></param>
        /// <returns></returns>
        public static string GetLeftOf(this string s, string leftOf)
        {
            Ensure.That(s).IsNotNull();

            int index = s.IndexOf(leftOf);

            if (index > -1)
            {
                return s.Substring(0, index);
            }

            return s;
        }

        /// <summary>
        /// Right 'x' characters of string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string s, int length)
        {
            Ensure.That(s).IsNotNull();

            if (s.Length < length)
            {
                return s;
            }

            return s.Substring(s.Length - length);
        }

        /// <summary>
        /// Gets the right of a specified string, or the string if NOT found
        /// </summary>
        /// <param name="s"></param>
        /// <param name="rightOf"></param>
        /// <returns></returns>
        public static string GetRightOf(this string s, string rightOf)
        {
            Ensure.That(s).IsNotNull();

            int index = s.IndexOf(rightOf);

            if (index > -1)
            {
                return s.Substring(index + rightOf.Length);
            }

            return s;
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

        /// <summary>
        /// Case insensitive StartsWith shortcut
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startsWith"></param>
        /// <returns></returns>
        public static bool StartsWithSameAs(this string s, string startsWith)
        {
            Ensure.That(s).IsNotNull();
            Ensure.That(startsWith).IsNotNull();

            return s.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Shortcut for !StartsWithSameAs
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startsWith"></param>
        /// <returns></returns>
        public static bool StartsWithNotSameAs(this string s, string startsWith)
        {
            return !StartsWithSameAs(s, startsWith);
        }

        /// <summary>
        /// Case insensitive EndsWith shortcut
        /// </summary>
        /// <param name="s"></param>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static bool EndsWithSameAs(this string s, string endsWith)
        {
            Ensure.That(s).IsNotNull();
            Ensure.That(endsWith).IsNotNull();

            return s.EndsWith(endsWith, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Shortcut for !EndsWithSameAs
        /// </summary>
        /// <param name="s"></param>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static bool EndsWithNotSameAs(this string s, string endsWith)
        {
            return !EndsWithSameAs(s, endsWith);
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

            lock (_stringToEnum)
            {
                if (!_stringToEnum.ContainsKey(t))
                {
                    _stringToEnum.Add(t, new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase));
                }

                if (!_stringToEnum[t].ContainsKey(stringOrDescription))
                {
                    _stringToEnum[t].Add(stringOrDescription, EnumExtensions.GetEnumObject(typeof(T), stringOrDescription));
                }
            }

            return (T)_stringToEnum[t][stringOrDescription];
        }

        #endregion
    }
}
