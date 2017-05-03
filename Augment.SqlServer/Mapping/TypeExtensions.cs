using System;

namespace Augment.SqlServer.Mapping
{
    static class TypeExtensions
    {
        public static bool IsPotentialPrimitive(this Type type)
        {
            return !IsPotentialMappableClass(type);
        }

        public static bool IsPotentialMappableClass(this Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }

            if (type == typeof(byte[]))
            {
                return false;
            }

            if (type == typeof(Guid))
            {
                return false;
            }

            if (type.IsPrimitive)
            {
                return false;
            }

            return true;
        }

        public static object DefaultValue(this Type type)
        {
            if (type.IsClass)
            {
                return null;
            }

            return Activator.CreateInstance(type);
        }
    }
}
