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
    internal static class ProjectionBuilder
    {
        private static readonly Type DictionaryType;
        private static readonly MethodInfo AddMethod;
        
        static ProjectionBuilder()
        {
            DictionaryType = typeof(Dictionary<string, object>);
            AddMethod = DictionaryType.GetMethod("Add") ??
                        throw new InvalidOperationException(
                            "Signature of Add method inside Dictionary class was changed.");
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

            var layeredSelectors = selectors.Select(selector => new LayeredSelector
                (
                    selector.Target.Split(separator: '.', StringSplitOptions.RemoveEmptyEntries),
                    selector
                ))
                .ToList();

            if (layeredSelectors.Count == 0)
            {
                return source.Cast<object>();
            }

            var dictionary = Expression.New(DictionaryType);

            var zeroLayer = InitializeLayer(parameter, dictionary, layeredSelectors, layer: 0);

            var lambda = Expression.Lambda(zeroLayer, parameter);

            var genericSelect = FastReflection.ForQueryable.Select<T, Dictionary<string, object>>();

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

                if (selectors.Count == 1 && selectors.First().Layers.Length == layer + 1)
                {
                    value = source.Property(selectors.First().Selector.Source);
                }
                else
                {
                    var nestedDictionary = Expression.New(DictionaryType);
                    value = InitializeLayer(source, nestedDictionary, selectors.ToList(), layer + 1);
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
            public LayeredSelector(string[] layers, ISelector selector)
            {
                Layers = layers;
                Selector = selector;
            }

            internal string[] Layers { get; }
            internal ISelector Selector { get; }
        }
    }
}