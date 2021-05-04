using System;
using System.Reflection;
using QueryNinja.Core;

namespace QueryNinja.Sources.GraphQL.SchemaGeneration
{
    /// <summary>
    /// Represent action method that contains <see cref="IQuery"/> argument.
    /// </summary>
    public class QueryRoot
    {
        /// <summary>
        /// Creates a new instance of <see cref="QueryRoot"/>
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="controllerType"></param>
        /// <param name="actionMethod"></param>
        /// <param name="queryType"></param>
        public QueryRoot(Type modelType, Type controllerType, MethodInfo actionMethod, Type queryType)
        {
            ModelType = modelType ?? throw new ArgumentNullException(nameof(modelType));
            ControllerType = controllerType ?? throw new ArgumentNullException(nameof(controllerType));
            ActionMethod = actionMethod ?? throw new ArgumentNullException(nameof(actionMethod));
            QueryType = queryType ?? throw new ArgumentNullException(nameof(queryType));
        }

        /// <summary>
        /// Represent target type for query.
        /// </summary>
        public Type ModelType { get; }

        /// <summary>
        /// Type of a controller that contains action method.
        /// </summary>
        public Type ControllerType { get; }

        /// <summary>
        /// Action method itself.
        /// </summary>
        public MethodInfo ActionMethod { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type QueryType { get; }
    }
}