using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QueryNinja.Core.Projection;
using QueryNinja.Targets.Queryable.Reflection;

namespace QueryNinja.Targets.Queryable.Projection
{
    /// <summary>
    /// Applies a projection from <see cref="IQueryable{T}"/> to structure defined by <see cref="ISelector"/>
    /// </summary>
    // todo: selectors model refactoring? as currently selectors are not hierarchical, which create problems.
    internal static class ProjectionBuilder
    {
        private static readonly Type DictionaryType;
        private static readonly MethodInfo AddMethod;

        static ProjectionBuilder()
        {
            DictionaryType = typeof(Dictionary<string, object>);
            AddMethod = DictionaryType.GetMethod("Add")!;
        }

        /// <summary>
        /// Applies <paramref name="selectors"/> on top of <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Source IQueryable</param>
        /// <param name="selectors">Collection of selectors.</param>
        /// <returns>Dynamic <see cref="IQueryable{T}"/>.</returns>
        internal static IQueryable<dynamic> Project<T>(this IQueryable<T> source, IEnumerable<ISelector> selectors)
        {
            var parameter = Expression.Parameter(typeof(T));

            //for selecting through lists layers should be built over source
            var layeredSelectors = selectors.Select(selector => new LayeredSelector
                (
                    selector.Source.Split(separator: '.')
                ))
                .ToList();

            if (layeredSelectors.Count == 0)
            {
                return source.Cast<object>();
            }

            var dictionary = Expression.New(DictionaryType);

            var zeroLayer = InitializeLayer(parameter, dictionary, layeredSelectors, layer: 0);

            var lambda = Expression.Lambda(zeroLayer, parameter);

            var genericSelect = FastReflection.ForQueryable<T>.Select();

            var queryBody = Expression.Call(genericSelect,
                source.Expression, Expression.Quote(lambda));

            var result = source.Provider.CreateQuery<dynamic>(queryBody);

            return result;
        }

        private static ListInitExpression InitializeLayer(
            Expression source,
            NewExpression dictionary,
            IEnumerable<LayeredSelector> layeredSelectors,
            int layer)
        {
            var groupedSelectors = layeredSelectors.GroupBy(selector => selector.Layers[layer]);

            var elementInits = new List<ElementInit>();

            foreach (var group in groupedSelectors)
            {
                var selectors = @group.ToList();
                Expression value;

                if (selectors.Count == 1 && selectors[index: 0].Layers.Length == layer + 1)
                {
                    //here selector should be executed. In this place method call selector should be used.
                    value = source.Property(selectors[index: 0].ToString());
                }
                else
                {
                    //get property of the next layer
                    var pathSegments = selectors[index: 0]
                        .Layers
                        .AsSpan()
                        .Slice(start: 0, layer + 1);

                    var pathToLayer = string.Join(separator: '.', pathSegments.ToArray());

                    var propertyExpression = source.Property(pathToLayer);

                    var collectionInterface = propertyExpression.Type.GetInterface("IEnumerable`1");

                    if (collectionInterface == null)
                    {
                        var nestedDictionary = Expression.New(DictionaryType);
                        value = InitializeLayer(source, nestedDictionary, selectors, layer + 1);
                    }
                    else
                    {
                        //in case our next layer is a list -> new list should be created
                        var collectionProperty = collectionInterface.GetGenericArguments()[0];

                        var nestedParameter = Expression.Parameter(collectionProperty);

                        var nestedDictionary = Expression.New(DictionaryType);

                        var nestedSelectors = selectors
                            .Select(selector => selector.Slice(layer))
                            .ToList();

                        value = InitializeLayer(nestedParameter, nestedDictionary, nestedSelectors, layer: 0);

                        var selectExpression = Expression.Lambda(value, nestedParameter);

                        var selectMethod = FastReflection.ForEnumerable.Select(collectionProperty);
                        value = Expression.Call(selectMethod, propertyExpression, selectExpression);
                    }
                }

                value = Expression.Convert(value, typeof(object));
                var targetPropertyName = Expression.Constant(@group.Key);
                var init = Expression.ElementInit(AddMethod, targetPropertyName, value);
                elementInits.Add(init);
            }

            var dictionaryInit = Expression.ListInit(dictionary, elementInits);

            return dictionaryInit;
        }

        private class LayeredSelector
        {
            public LayeredSelector(string[] layers)
            {
                Layers = layers;
            }

            internal string[] Layers { get; }

            public LayeredSelector Slice(int layer)
            {
                var slicedLayers = Layers.AsSpan().Slice(layer + 1).ToArray();
                return new LayeredSelector(slicedLayers);
            }

            /// <summary>
            /// Builds path for all layers.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Join(separator: '.', Layers);
            }
        }
    }
}