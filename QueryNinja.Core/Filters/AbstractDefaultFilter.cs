using System;

namespace QueryNinja.Core.Filters
{
    /// <summary>
    ///   Applies filter defined by the <see cref="AbstractDefaultFilter{TOperation}.Operation" />
    /// </summary>
    public abstract class AbstractDefaultFilter<TOperation> : IDefaultFilter<TOperation>
        where TOperation : struct, Enum
    {
        /// <summary>
        ///   Default constructor for Filter instance creation.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        protected AbstractDefaultFilter(TOperation operation, string property, string value)
        {
            Operation = operation;
            Property = property;
            Value = value;
        }

        /// <inheritdoc />
        public TOperation Operation { get; }

        /// <inheritdoc />
        public string Property { get; }

        /// <inheritdoc />
        public string Value { get; }

        /// <summary>
        ///   Debug-friendly implementation. Return string representation of filter operation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Property} {Operation} {Value}";
        }
    }
}