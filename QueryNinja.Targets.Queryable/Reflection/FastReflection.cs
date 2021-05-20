using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QueryNinja.Targets.Queryable.Reflection
{
    internal static class FastReflection
    {
        public static class ForEnumerable
        {
            private static readonly MethodInfo ContainsMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(methodInfo => methodInfo.Name == "Contains" && methodInfo.GetParameters().Length == 2);

            private static readonly MethodInfo AnyMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(methodInfo => methodInfo.Name == "Any" && methodInfo.GetParameters().Length == 1);

            private static readonly MethodInfo SelectMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(methodInfo => methodInfo.Name == "Select" && methodInfo.GetParameters().Length == 2);

            private static readonly ConcurrentDictionary<Type, MethodInfo> GenericAnyCache = new ConcurrentDictionary<Type, MethodInfo>();
            private static readonly ConcurrentDictionary<Type, MethodInfo> GenericContainsCache = new ConcurrentDictionary<Type, MethodInfo>();
            private static readonly ConcurrentDictionary<Type, MethodInfo> GenericSelectCache = new ConcurrentDictionary<Type, MethodInfo>();

            public static MethodInfo Any(Type elementType)
            {
                if (GenericAnyCache.TryGetValue(elementType, out var result))
                {
                    return result;
                }

                var genericAny = AnyMethod.MakeGenericMethod(elementType);
                GenericAnyCache.TryAdd(elementType, genericAny);
                return genericAny;
            }

            public static MethodInfo Contains(Type elementType)
            {
                if (GenericContainsCache.TryGetValue(elementType, out var result))
                {
                    return result;
                }
                var genericContains = ContainsMethod.MakeGenericMethod(elementType);
                GenericContainsCache.TryAdd(elementType, genericContains);
                return genericContains;
            }

            public static MethodInfo Select(Type elementType)
            {
                if (GenericSelectCache.TryGetValue(elementType, out var result))
                {
                    return result;
                }
                var genericContains = SelectMethod.MakeGenericMethod(elementType, typeof(Dictionary<string, object>));
                GenericSelectCache.TryAdd(elementType, genericContains);
                return genericContains;
            }
        }

        public static class ForQueryable<T>
        {
            private static readonly MethodInfo WhereMethod = typeof(System.Linq.Queryable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(methodInfo => methodInfo.Name == "Where" && methodInfo.GetParameters()
                    .Last()
                    .ParameterType.GetGenericArguments()
                    .Last()
                    .GetGenericArguments()
                    .Length == 2)
                .MakeGenericMethod(typeof(T));

            private static readonly MethodInfo SelectMethod = typeof(System.Linq.Queryable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(methodInfo => methodInfo.Name == "Select" && methodInfo.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), typeof(Dictionary<string, object>));
            
            public static MethodInfo Where()
            {
                return WhereMethod;
            }

            public static MethodInfo Select()
            {
                return SelectMethod;
            }
        }
    }
}