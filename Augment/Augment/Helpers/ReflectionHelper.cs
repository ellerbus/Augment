using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using EnsureThat;

namespace Augment
{
    #region Exception

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ReflectionHelperException : Exception, ISerializable
    {
        internal const string CannotFindMessage = "Specified Property cannot be found '{0}->{1}'";
        internal const string CannotReadMessage = "Specified Property cannot be read from '{0}->{1}'";
        internal const string CannotWriteMessage = "Specified Property cannot be written to '{0}->{1}'";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="property"></param>
        public ReflectionHelperException(string message, Type type, string property)
            : base(message.FormatArgs(type.FullName, property))
        {
            ReflectedType = type;
            PropertyName = property;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ReflectionHelperException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ReflectedType = (Type)info.GetValue("ReflectedType", typeof(Type));
            PropertyName = info.GetString("PropertyName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Ensure.That(info, "info").IsNotNull();

            info.AddValue("PropertyName", PropertyName);

            info.AddValue("ReflectedType", ReflectedType, typeof(Type));

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Type being reflected for
        /// </summary>
        public Type ReflectedType { get; private set; }

        /// <summary>
        /// Property Name not found
        /// </summary>
        public string PropertyName { get; private set; }
    }

    #endregion

    /// <summary>
    /// Reflection Helper for getting/setting properties (v1)
    /// </summary>
    public static class ReflectionHelper
    {
        #region Members

        private static ConcurrentDictionary<Type, PropertyWrapper> _wrappers = new ConcurrentDictionary<Type, PropertyWrapper>();

        class PropertyWrapper
        {
            class Methods
            {
                public MethodBase Get { get; set; }
                public MethodBase Set { get; set; }
            }

            private Type _type;

            private Dictionary<string, Methods> _properties = new Dictionary<string, Methods>();

            public PropertyWrapper(Type type)
            {
                _type = type;

                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (!pi.CanRead && !pi.CanWrite)
                    {
                        continue;
                    }

                    Methods mappings = new Methods();

                    if (pi.CanRead)
                    {
                        mappings.Get = pi.GetGetMethod();
                    }

                    if (pi.CanWrite)
                    {
                        mappings.Set = pi.GetSetMethod();
                    }

                    _properties.Add(pi.Name, mappings);
                }
            }

            public object GetValue(object instance, string name)
            {
                Methods mappings = GetMapping(instance, name);

                if (mappings.Get != null)
                {
                    return mappings.Get.Invoke(instance, null);
                }

                throw new ReflectionHelperException(ReflectionHelperException.CannotReadMessage, instance.GetType(), name);
            }

            public void SetValue(object instance, string name, object value)
            {
                Methods mappings = GetMapping(instance, name);

                if (mappings.Set != null)
                {
                    mappings.Set.Invoke(instance, new[] { value });

                    return;
                }

                throw new ReflectionHelperException(ReflectionHelperException.CannotWriteMessage, instance.GetType(), name);
            }

            private Methods GetMapping(object instance, string name)
            {
                Methods mappings = null;

                if (_properties.TryGetValue(name, out mappings))
                {
                    return mappings;
                }

                throw new ReflectionHelperException(ReflectionHelperException.CannotFindMessage, instance.GetType(), name);
            }
        }

        #endregion

        #region Methods

        private static PropertyWrapper GetPropertyWrapper(Type t)
        {
            return _wrappers.GetOrAdd(t, x => new PropertyWrapper(x));
        }

        /// <summary>
        /// Gets the value of a property for a given instance of an object
        /// </summary>
        /// <param name="instance">Instance of an object</param>
        /// <param name="propertyName">Name of Property to Get value from</param>
        /// <returns></returns>
        public static object GetValueOfProperty(object instance, string propertyName)
        {
            Ensure.That(instance, "instance").IsNotNull();
            Ensure.That(propertyName, "propertyName").IsNotNull();

            PropertyWrapper pw = GetPropertyWrapper(instance.GetType());

            return pw.GetValue(instance, propertyName);
        }

        /// <summary>
        /// Gets the value of a property path (ie. A.B.C) for a given instance of an object
        /// </summary>
        /// <param name="instance">Instance of an object</param>
        /// <param name="propertyPath">Path to Property to get value from</param>
        /// <returns></returns>
        public static object GetValueOfPropertyPath(object instance, string propertyPath)
        {
            Ensure.That(instance, "instance").IsNotNull();
            Ensure.That(propertyPath, "propertyPath").IsNotNull();

            PropertyWrapper pw = GetPropertyWrapper(instance.GetType());

            object value = instance;

            string[] paths = propertyPath.Split('.');

            for (int x = 0; x < paths.Length; x++)
            {
                value = GetValueOfProperty(value, paths[x]);
            }

            return value;
        }

        /// <summary>
        /// Sets the value of a property for a given instance of an object
        /// </summary>
        /// <param name="instance">Instance of an object</param>
        /// <param name="propertyName">Name of Property to Set</param>
        /// <param name="value">Value to use for setting</param>
        /// <returns></returns>
        public static void SetValueOfProperty(object instance, string propertyName, object value)
        {
            Ensure.That(instance, "instance").IsNotNull();
            Ensure.That(propertyName, "propertyName").IsNotNull();

            PropertyWrapper pw = GetPropertyWrapper(instance.GetType());

            pw.SetValue(instance, propertyName, value);
        }

        /// <summary>
        /// Sets the value of a property path (ie. A.B.C) for a given instance of an object
        /// </summary>
        /// <param name="instance">Instance of an object</param>
        /// <param name="propertyPath">Path to Property to set value for</param>
        /// <param name="value">Value to use for setting</param>
        /// <returns></returns>
        public static void SetValueOfPropertyPath(object instance, string propertyPath, object value)
        {
            Ensure.That(instance, "instance").IsNotNull();
            Ensure.That(propertyPath, "propertyPath").IsNotNull();

            object valueOfProperty = instance;

            string[] paths = propertyPath.Split('.');

            for (int x = 0; x < paths.Length - 1; x++)
            {
                valueOfProperty = GetValueOfProperty(valueOfProperty, paths[x]);
            }

            PropertyWrapper pw = GetPropertyWrapper(valueOfProperty.GetType());

            pw.SetValue(valueOfProperty, paths[paths.Length - 1], value);
        }

        #endregion
    }
}
