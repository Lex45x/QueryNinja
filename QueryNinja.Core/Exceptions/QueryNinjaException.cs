using System;

namespace QueryNinja.Core.Exceptions
{
    /// <summary>
    /// Base exception for all QueryNinja exceptions.
    /// </summary>
    public abstract class QueryNinjaException : Exception
    {
        protected QueryNinjaException(string message)
            : base(message)
        {
        }

        protected QueryNinjaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}