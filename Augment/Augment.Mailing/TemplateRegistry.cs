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

        #endregion

        #region Template Methods

        public static string RenderWith<T>(T item, string template)
        {
            lock (_lock)
            {
                int key = template.GetHashCode();

                Template tmpl = null;

                if (!_templates.TryGetValue(key, out tmpl))
                {
                    tmpl = Template.Parse(template);

                    _templates.Add(key, tmpl);
                }

                AddAsSafe<T>();

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

        private static IEnumerable<PropertyInfo> PropertiesOf(Type t)
        {
            foreach (PropertyInfo p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                yield return p;
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
                }
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
}
