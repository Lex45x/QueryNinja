using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QueryNinja.Targets.Queryable.Exceptions;

namespace QueryNinja.Targets.Queryable
{
    /// <summary>
    ///   Contains extensions to work with expression trees.
    /// </summary>
    public static class ExpressionsExtensions
    {
        /// <summary>
        ///   Creates a constant expression of desired type from string value. <br />
        ///   Includes built-in type conversion.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <exception cref="TypeConversionException">
        ///   When <see cref="TypeDescriptor" /> unable to convert string
        ///   <paramref name="value" /> to instance of type <paramref name="type" />
        /// </exception>
        /// <returns></returns>
        public static Expression AsConstant(this string value, Type type)
        {
            if (type == typeof(string))
            {
                return Expression.Constant(value);
            }

            var typeConverter = TypeDescriptor.GetConverter(type);

            object? converted;
            try
            {
                converted = typeConverter.ConvertFromString(value);
            }
            catch (Exception e)
            {
                throw new TypeConversionException(value, type, e);
            }

            var constantExpression = Expression.Constant(converted, type);

            return constantExpression;
        }

        /// <summary>
        ///   Takes desired property with <paramref name="path" /> from <typeparamref name="TEntity" />
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="path"></param>
        /// <exception cref="InvalidPropertyException">
        ///   In case of at leas one property in the <paramref name="path" /> will not be
        ///   found.
        /// </exception>
        /// <returns></returns>
        public static LambdaExpression From<TEntity>(this string path)
        {
            var pathSegments = path.Split(separator: '.');

            var parameter = Expression.Parameter(typeof(TEntity));

            Expression currentProperty = pathSegments
                .Aggregate<string, Expression>(parameter, (current, property) =>
                {
                    //implementation from Expression.Property(string, Type, string);
                    //needed to add custom error handling on top of Expressions
                    var propertyInfo = current.Type.GetProperty(property,
                                           BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                           BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy)
                                       ?? current.Type.GetProperty(property,
                                           BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic |
                                           BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);

                    if (propertyInfo == null)
                    {
                        throw new InvalidPropertyException(path, current.Type, property);
                    }

                    return Expression.Property(current, propertyInfo);
                });

            return Expression.Lambda(currentProperty, parameter);
        }

        /// <summary>
        ///   Allows to take property from parameter expression.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns>Null of property with this name was not found, otherwise <see cref="MemberExpression" /></returns>
        public static Expression? TryGetProperty(this Expression instance, string name)
        {
            var properties = name.Split(separator: '.');

            Expression result = instance;

            result = properties.Aggregate(result, (expression, property) =>
            {
                try
                {
                    return Expression.Property(expression, property);
                }
                catch
                {
                    return null!;
                }
            });

            return result;
        }

        /// <summary>
        ///   Allows to call <paramref name="method" /> on <paramref name="instance" /> with supplied <paramref name="arguments" />
        /// </summary>
        /// <param name="instance">Instance that contains desired method.</param>
        /// <param name="method">Method name</param>
        /// <param name="arguments">Parameters with proper naming</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach", Justification = "Performance optimizations")]
        public static Expression Call(this Expression instance, string method,
            IReadOnlyDictionary<string, string>? arguments)
        {
            var methodInfo = instance.Type.GetMethod(method,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (methodInfo == null)
            {
                throw new NotImplementedException();
            }

            var parameters = methodInfo.GetParameters();
            var constants = new List<Expression>(parameters.Length);

            for (var parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                var parameter = parameters[parameterIndex];

                var value = arguments?[parameter.Name];

                constants.Add(value.AsConstant(parameter.ParameterType));
            }

            return Expression.Call(instance, methodInfo, constants);
        }

        /// <summary>
        ///   Checks whether Queryable already contains order expressions defined.
        /// </summary>
        /// <param name="queryable">Source</param>
        /// <returns></returns>
        public static bool IsOrderExpressionDefined(this IQueryable queryable)
        {
            var methodCall = queryable.Expression as MethodCallExpression;

            while (methodCall != null)
            {
                if (methodCall.Method.Name.Contains("OrderBy") || methodCall.Method.Name.Contains("ThenBy"))
                {
                    return true;
                }

                methodCall = methodCall.Arguments.OfType<MethodCallExpression>().FirstOrDefault();
            }

            return false;
        }
    }
}