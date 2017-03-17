using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotLiquid;

namespace Augment.Mailing
{
    static class TemplateRegistry
    {
        #region Members

        private static Dictionary<int, Template> _templates = new Dictionary<int, Template>();

        private static HashSet<Type> _types = new HashSet<Type>();

        private static readonly object _lock = new object();

        static TemplateRegistry()
        {
            Template.RegisterFilter(typeof(LiquidFilters));
        }

        #endregion

        #region Template Methods

        public static string RenderWith<T>(T item, string template)
        {
            lock (_lock)
            {
                AddAsSafe<T>();

                int key = template.GetHashCode();

                Template tmpl = null;

                if (!_templates.TryGetValue(key, out tmpl))
                {
                    tmpl = Template.Parse(template);

                    _templates.Add(key, tmpl);
                }

                return tmpl.Render(Hash.FromAnonymousObject(item));
            }
        }

        #endregion

        #region Reflection Methods

        private static void AddAsSafe<T>()
        {
            lock (_lock)
            {
                Type t = typeof(T);

                AddAsSafe(t);
            }
        }

        private static void AddAsSafe(Type t)
        {
            lock (_lock)
            {
                if (!_types.Contains(t))
                {
                    IList<PropertyInfo> properties = PropertiesOf(t).ToList();

                    AddAsSafe(t, properties);

                    foreach (PropertyInfo p in properties)
                    {
                        Type pt = p.PropertyType;

                        if (!pt.IsValueType && pt != typeof(string))
                        {
                            AddAsSafe(pt);
                        }
                    }

                    if (t.IsGenericType)
                    {
                        foreach (Type x in t.GetGenericArguments())
                        {
                            AddAsSafe(x);
                        }
                    }

                    if (t.IsArray)
                    {
                        AddAsSafe(t.GetElementType());
                    }
                }
            }
        }

        private static IEnumerable<PropertyInfo> PropertiesOf(Type t)
        {
            foreach (PropertyInfo p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                yield return p;
            }
        }

        private static void AddAsSafe(Type t, IList<PropertyInfo> properties)
        {
            lock (_lock)
            {
                string[] names = properties.Select(x => x.Name).ToArray();

                Template.RegisterSafeType(t, names);

                _types.Add(t);
            }
        }

        #endregion
    }

    /// <summary>
    /// Enhancements to existing liquid filters
    /// </summary>
    public static class LiquidFilters
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(object input, string format)
        {
            if (input == null)
            {
                return null;
            }

            if (format.IsNullOrWhiteSpace())
            {
                return input.ToString();
            }

            return string.Format("{0:" + format + "}", input);
        }
    }
}
