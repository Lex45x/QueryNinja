using QueryNinja.Core.Filters;

namespace QueryNinja.Targets.EntityFrameworkCore.Filters
{
    /// <summary>
    /// Allows to use DB-related filters inside IQuery.
    /// </summary>
    public class DatabaseFunctionFilter : AbstractDefaultFilter<DatabaseFunction>
    {
        /// <summary>
        /// Creates filter instance.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public DatabaseFunctionFilter(DatabaseFunction operation, string property, string value) 
            : base(operation, property, value)
        {
        }
    }
}