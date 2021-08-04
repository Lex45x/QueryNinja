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
            AddMethod = DictionaryType.GetMethod("Add")!;
        }

        /// <summary>
        /// Applies <paramref name="selectors"/> on top of <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Source IQueryable</param>
        /// <param name="selectors">Collection of selectors.</param>
        /// <returns>Dynamic <see cref="IQueryable{T}"/>.</returns>
        internal static IQueryable<dynamic> Project<T>(this IQueryable<T> source, IReadOnlyList<ISelector> selectors)
        {
            var parameter = Expression.Parameter(typeof(T));

            if (selectors.Count == 0)
            {
                return source.Cast<object>();
            }

            var dictionary = Expression.New(DictionaryType);

            var zeroLayer = InitializeLayer(parameter, dictionary, selectors);

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
            IEnumerable<ISelector> selectors)
        {
            var elementInits = new List<ElementInit>();

            foreach (var selector in selectors)
            {
                Expression value;

                if (selector.NestedSelectors.Count == 0)
                {
                    value = BuildExpression(source, selector);
                }
                else
                {
                    var propertyExpression = BuildExpression(source, selector);

                    var collectionInterface = propertyExpression.Type.GetInterface("IEnumerable`1");

                    if (collectionInterface == null)
                    {
                        var nestedDictionary = Expression.New(DictionaryType);
                        value = InitializeLayer(source, nestedDictionary, selector.NestedSelectors);
                    }
                    else
                    {
                        //in case our next layer is a list -> new list should be created
                        var collectionProperty = collectionInterface.GetGenericArguments()[0];

                        var nestedParameter = Expression.Parameter(collectionProperty);

                        var nestedDictionary = Expression.New(DictionaryType);

                        value = InitializeLayer(nestedParameter, nestedDictionary, selector.NestedSelectors);

                        var selectExpression = Expression.Lambda(value, nestedParameter);

                        var selectMethod = FastReflection.ForEnumerable.Select(collectionProperty);
                        value = Expression.Call(selectMethod, propertyExpression, selectExpression);
                    }
                }

                value = Expression.Convert(value, typeof(object));
                var targetPropertyName = Expression.Constant(selector.Source);
                var init = Expression.ElementInit(AddMethod, targetPropertyName, value);
                elementInits.Add(init);
            }

            var dictionaryInit = Expression.ListInit(dictionary, elementInits);

            return dictionaryInit;
        }

        private static Expression BuildExpression(Expression source, ISelector selector)
        {
            Expression value = (selector) switch
            {
                ExecuteSelector executeSelector => source.Call(executeSelector.Source, executeSelector.Arguments),
                Selector _ => source.Property(selector.Source),
                _ => throw new ArgumentOutOfRangeException(nameof(selector), selector,
                    "This type of selectors is currently not supported.")
            };

            return value;
        }
    }
}