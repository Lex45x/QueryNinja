using System;

namespace QueryNinja.Core.Exceptions
{
    /// <summary>
    /// Base exception for all QueryNinja exceptions.
    /// </summary>
    public abstract class QueryNinjaException : Exception
    {
        /// <inheritdoc />
        protected QueryNinjaException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        protected QueryNinjaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}