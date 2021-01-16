using System;

namespace QueryNinja.Targets.Queryable.Exceptions
{
    /// <summary>
    /// Query building in <see cref="BuilderType"/> is failed with <see cref="QueryBuildingException.InnerException"/>.
    /// </summary>
    public class QueryBuildingException : QueryableTargetException
    {
        /// <summary>
        /// Type of the builder that failed building query.
        /// </summary>
        public Type BuilderType { get; }

        /// <inheritdoc />
        public QueryBuildingException(Type builderType, Exception innerException)
            : base("Query building is failed", innerException)
        {
            BuilderType = builderType;
        }
    }
}