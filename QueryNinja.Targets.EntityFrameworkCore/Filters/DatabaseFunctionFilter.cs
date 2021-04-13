using QueryNinja.Core.Filters;

namespace QueryNinja.Targets.EntityFrameworkCore.Filters
{
    /// <summary>
    /// Allows to use DB-related filters inside IQuery.
    /// </summary>
    public class DatabaseFunctionFilter : IDefaultFilter<DatabaseFunction>
    {
        /// <summary>
        /// Creates filter instance.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
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