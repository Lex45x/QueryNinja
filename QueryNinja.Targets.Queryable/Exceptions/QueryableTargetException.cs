using System;
using QueryNinja.Core.Exceptions;

namespace QueryNinja.Targets.Queryable.Exceptions
{
    /// <summary>
    ///   Base exception for all exceptions in QueryNinja.Targets.Queryable namespace.
    /// </summary>
    public abstract class QueryableTargetException : QueryNinjaException
    {
        /// <inheritdoc />
        protected QueryableTargetException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        protected QueryableTargetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}