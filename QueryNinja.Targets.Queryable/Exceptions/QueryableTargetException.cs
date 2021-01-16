using System;
using QueryNinja.Core.Exceptions;

namespace QueryNinja.Targets.Queryable.Exceptions
{
    public abstract class QueryableTargetException : QueryNinjaException
    {
        protected QueryableTargetException(string message)
            : base(message)
        {
        }

        protected QueryableTargetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}