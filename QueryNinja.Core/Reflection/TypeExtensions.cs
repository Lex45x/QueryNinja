using System;
using System.Linq;

namespace QueryNinja.Core.Reflection
{
    public static class TypeExtensions
    {
        public static string GetFriendlyTypeName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var baseName = type.Name.Remove(type.Name.IndexOf(value: '`'));

            if (type.IsGenericTypeDefinition)
            {
                return baseName;
            }

            return type.GetGenericArguments()
                .Aggregate(baseName, (name, argument) => name + GetFriendlyTypeName(argument));
        }
    }
}