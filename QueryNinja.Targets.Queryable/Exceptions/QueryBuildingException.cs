using System;

namespace QueryNinja.Targets.Queryable.Exceptions
{
    /// <summary>
    /// Query building in <see cref="BuilderType"/> is failed with <see cref="QueryBuildingException.InnerException"/>.
    /// </summary>
    public class QueryBuildingException : QueryableTargetException
    {
        public Type BuilderType { get; }

        public QueryBuildingException(Type builderType, Exception innerException)
            : base("Query building is failed", innerException)
        {
            BuilderType = builderType;
        }
    }
}