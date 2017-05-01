using System;

namespace Augment.SqlServer.Mapping
{
    static class TypeExtensions
    {
        public static bool IsPotentialMappableClass(this Type type)
        {
            return type.IsClass && type != typeof(string);
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
