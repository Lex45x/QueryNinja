using System;
using System.Collections.Concurrent;
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

            private static readonly ConcurrentDictionary<Type, MethodInfo> GenericAnyCache = new ConcurrentDictionary<Type, MethodInfo>();
            private static readonly ConcurrentDictionary<Type, MethodInfo> GenericContainsCache = new ConcurrentDictionary<Type, MethodInfo>();
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
        }

        public static class ForQueryable
        {
            private static readonly MethodInfo WhereMethod = typeof(System.Linq.Queryable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(methodInfo => methodInfo.Name == "Where" && methodInfo.GetParameters()
                    .Last()
                    .ParameterType.GetGenericArguments()
                    .Last()
                    .GetGenericArguments()
                    .Length == 2);

            private static readonly MethodInfo SelectMethod = typeof(System.Linq.Queryable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(methodInfo => methodInfo.Name == "Select" && methodInfo.GetParameters().Length == 2);

            private static readonly ConcurrentDictionary<Type, MethodInfo> GenericWhereCache = new ConcurrentDictionary<Type, MethodInfo>();
            private static readonly ConcurrentDictionary<(Type,Type), MethodInfo> GenericSelectCache = new ConcurrentDictionary<(Type, Type), MethodInfo>();


            public static MethodInfo Where<T>()
            {
                if (GenericWhereCache.TryGetValue(typeof(T), out var result))
                {
                    return result;
                }

                var genericWhere = WhereMethod.MakeGenericMethod(typeof(T));
                GenericWhereCache.TryAdd(typeof(T), genericWhere);
                return genericWhere;
            }

            public static MethodInfo Select<TSource,TResult>()
            {
                if (GenericSelectCache.TryGetValue((typeof(TSource), typeof(TResult)), out var result))
                {
                    return result;
                }

                var genericSelect = SelectMethod.MakeGenericMethod(typeof(TSource), typeof(TResult));
                GenericSelectCache.TryAdd((typeof(TSource), typeof(TResult)), genericSelect);
                return genericSelect;
            }
        }
    }
}