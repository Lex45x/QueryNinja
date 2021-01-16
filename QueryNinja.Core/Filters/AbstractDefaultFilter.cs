using System;

namespace QueryNinja.Core.Filters
{
    /// <summary>
    /// Applies filter defined by the <see cref="AbstractDefaultFilter{TOperation}.Operation"/>
    /// </summary>
    public abstract class AbstractDefaultFilter<TOperation> : IDefaultFilter<TOperation>
        where TOperation : struct, Enum
    {
        protected AbstractDefaultFilter(TOperation operation, string property, string value)
        {
            Operation = operation;
            Property = property;
            Value = value;
        }
        ///<inheritdoc/>
        public TOperation Operation { get; }

        ///<inheritdoc/>
        public string Property { get; }

        ///<inheritdoc/>
        public string Value { get; }

        public override string ToString()
        {
            return $"{Property} {Operation} {Value}";
        }
    }
}
