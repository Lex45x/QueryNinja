using System;
using System.Linq.Expressions;

namespace QueryNinja.Targets.Queryable.Exceptions
{
    /// <summary>
    /// Occurred when there is an attempt to create <see cref="MemberExpression"/> for missing property. 
    /// </summary>
    public class InvalidPropertyException : QueryableTargetException
    {
        /// <summary>
        /// Original path to the property.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Type, where <see cref="Property"/> was not found.
        /// </summary>
        public Type CurrentType { get; }

        /// <summary>
        /// Missing property name.
        /// </summary>
        public string Property { get; }

        /// <inheritdoc />
        public InvalidPropertyException(string path, Type currentType, string property)
            : base($"Unable to find property '{property}' in Type '{currentType}' when building a path '{path}'")
        {
            Path = path;
            CurrentType = currentType;
            Property = property;
        }
    }
}