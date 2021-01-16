using System;

namespace QueryNinja.Targets.Queryable.Exceptions
{
    /// <summary>
    /// Occurred when there is no way to create instance of <see cref="Type"/> from string <see cref="Value"/>
    /// </summary>
    public class TypeConversionException : QueryableTargetException
    {
        /// <summary>
        /// String value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Desired type for <see cref="Value"/> conversion.
        /// </summary>
        public Type Type { get; }

        /// <inheritdoc />
        public TypeConversionException(string value, Type type, Exception e)
            : base($"Cannot convert string '{value}' to {type}", e)
        {
            Value = value;
            Type = type;
        }
    }
}