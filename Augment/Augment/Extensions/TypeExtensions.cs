using System;
using System.Linq;
using System.Reflection;
using System.Text;
using EnsureThat;

namespace Augment
{
    /// <summary>
    /// Type Extensions
    /// </summary>
    public static class TypeExtensions
    {
        #region Type Assistance

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <remarks>
        ///// For instance:
        ///// Type customType = typeof(Custom<>);
        ///// 
        ///// type.IsSubclassOfGeneric(customType)
        ///// 
        ///// works when type is NOT generic
        ///// </remarks>
        ///// <param name="type"></param>
        ///// <param name="rawGenericType"></param>
        ///// <returns></returns>
        //public static bool IsSubclassOfGeneric(this Type type, Type rawGenericType)
        //{
        //    while (rawGenericType != null && rawGenericType != typeof(object))
        //    {
        //        Type cur = rawGenericType.IsGenericType ? rawGenericType.GetGenericTypeDefinition() : rawGenericType;

        //        if (type == cur)
        //        {
        //            return true;
        //        }

        //        rawGenericType = rawGenericType.BaseType;
        //    }

        //    return false;
        //}

        /// <summary>
        /// Is this type nullable?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType)
            {
                return true; // ref-type
            }

            if (Nullable.GetUnderlyingType(type) != null)
            {
                return true; // Nullable<T>
            }

            return false; // value-type
        }

        #endregion

        #region Getters

        /// <summary>
        /// Gets the assembly name
        /// </summary>
        /// <returns></returns>
        public static string GetNameOfAssembly(this Type type)
        {
            Ensure.That(type).IsNotNull();

            string nm = type.Assembly.GetName().FullName.Split(',').First();

            return nm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDescription(this Type type)
        {
            if (string.IsNullOrEmpty(type.FullName))
            {
                return type.Name;
            }

            StringBuilder sb = new StringBuilder(type.FullName.GetLeftOf("`"));

            if (type.IsGenericType)
            {
                sb.AppendFormat("<{0}>", type.GetGenericArguments().Select(x => x.GetDescription()).Join(","));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get a readable description for a method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetDescription(this MethodBase method)
        {
            return GetDescription(method as MethodInfo);
        }

        /// <summary>
        /// Get a readable description for a method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetDescription(this MethodInfo method)
        {
            StringBuilder sb = new StringBuilder(method.Name.GetLeftOf("`"));

            if (method.IsGenericMethod)
            {
                sb.AppendFormat("<{0}>", method.GetGenericArguments().Select(x => x.GetDescription()).Join(","));
            }

            sb.Append("(")
                .Append(method.GetParameters().Select(x => x.ParameterType.Name).Join(","))
                .Append(")")
                ;

            return sb.ToString();
        }

        #endregion
    }
}