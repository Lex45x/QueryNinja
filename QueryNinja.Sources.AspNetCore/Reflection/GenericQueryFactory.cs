using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using QueryNinja.Core;
using QueryNinja.Core.Projection;

namespace QueryNinja.Sources.AspNetCore.Reflection
{
    internal static class GenericQueryFactory
    {
        private static readonly ConcurrentDictionary<Type, DynamicQueryFactory> DynamicQueryFactories =
            new ConcurrentDictionary<Type, DynamicQueryFactory>();

        private delegate IDynamicQuery DynamicQueryFactory(IReadOnlyList<IQueryComponent> components,
            IReadOnlyList<ISelector> selectors);

        private static readonly ConcurrentDictionary<Type, QueryFactory> QueryFactories =
            new ConcurrentDictionary<Type, QueryFactory>();

        private delegate IQuery QueryFactory(IReadOnlyList<IQueryComponent> components);

        public static IDynamicQuery DynamicQuery(Type queryType, 
            IReadOnlyList<IQueryComponent> components,
            IReadOnlyList<ISelector> selectors)
        {
            if (DynamicQueryFactories.TryGetValue(queryType, out var factory))
            {
                return factory(components, selectors);
            }

            var modelType = queryType.GetGenericArguments()[0];
            var constructorInfo = typeof(DynamicQuery<>).MakeGenericType(modelType).GetConstructors().First();

            var componentsParameter = Expression.Parameter(typeof(IReadOnlyList<IQueryComponent>));
            var selectorsParameter = Expression.Parameter(typeof(IReadOnlyList<ISelector>));

            var newQueryInstance = Expression.New(constructorInfo, componentsParameter, selectorsParameter);

            var convertedInstance = Expression.Convert(newQueryInstance, typeof(IDynamicQuery));

            var expressionFactory = Expression.Lambda<DynamicQueryFactory>(convertedInstance, componentsParameter, selectorsParameter);

            var compiledFactory = expressionFactory.Compile();

            DynamicQueryFactories.TryAdd(queryType, compiledFactory);

            return compiledFactory(components, selectors);
        }

        public static IQuery Query(Type queryType, IReadOnlyList<IQueryComponent> components)
        {
            if (QueryFactories.TryGetValue(queryType, out var factory))
            {
                return factory(components);
            }

            var modelType = queryType.GetGenericArguments()[0];
            var constructorInfo = typeof(Query<>).MakeGenericType(modelType).GetConstructors().First();

            var componentsParameter = Expression.Parameter(typeof(IReadOnlyList<IQueryComponent>));

            var newQueryInstance = Expression.New(constructorInfo, componentsParameter);

            var convertedInstance = Expression.Convert(newQueryInstance, typeof(IQuery));

            var expressionFactory = Expression.Lambda<QueryFactory>(convertedInstance, componentsParameter);

            var compiledFactory = expressionFactory.Compile();

            QueryFactories.TryAdd(queryType, compiledFactory);

            return compiledFactory(components);
        }
    }
}