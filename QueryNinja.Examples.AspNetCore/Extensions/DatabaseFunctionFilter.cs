using QueryNinja.Core.Filters;

namespace QueryNinja.Examples.AspNetCore.Extensions
{
    /// <summary>
    /// Allows to use DB-related filters inside IQuery.
    /// </summary>
    public class DatabaseFunctionFilter : IDefaultFilter<DatabaseFunction>
    {
        public DatabaseFunctionFilter(DatabaseFunction operation, string property, string value)
        {
            Operation = operation;
            Property = property;
            Value = value;
        }

        /// <inheritdoc />
        public DatabaseFunction Operation { get; }

        /// <inheritdoc />
        public string Property { get; }

        /// <inheritdoc />
        public string Value { get; }
    }
}