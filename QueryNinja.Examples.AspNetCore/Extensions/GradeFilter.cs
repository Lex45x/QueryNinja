using System.Collections.Generic;
using QueryNinja.Core.Attributes;
using QueryNinja.Core.Filters;
using QueryNinja.Examples.AspNetCore.DbContext.Entities;

namespace QueryNinja.Examples.AspNetCore.Extensions
{
    /// <summary>
    ///   Allows to filter on top of <see cref="Student.Grades" />
    /// </summary>
    [DefinedFor(typeof(ICollection<Grade>))]
    public class GradeFilter : IDefaultFilter<GradeOperations>
    {
        /// <summary>
        ///   Default constructor.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public GradeFilter(GradeOperations operation, string property, string value)
        {
            Operation = operation;
            Property = property;
            Value = value;
        }

        /// <inheritdoc />
        public GradeOperations Operation { get; }

        /// <inheritdoc />
        public string Property { get; }

        /// <inheritdoc />
        public string Value { get; }
    }
}