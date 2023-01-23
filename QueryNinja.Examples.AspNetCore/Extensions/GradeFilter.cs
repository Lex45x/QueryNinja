using QueryNinja.Core.Filters;
using QueryNinja.Examples.AspNetCore.DbContext.Entities;

namespace QueryNinja.Examples.AspNetCore.Extensions
{
    /// <summary>
    /// Allows to filter on top of <see cref="Student.Grades"/>
    /// </summary>
    public class GradeFilter : AbstractDefaultFilter<GradeOperations>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public GradeFilter(GradeOperations operation, string property, string value)
            : base(operation, property, value)
        {
        }
    }
}