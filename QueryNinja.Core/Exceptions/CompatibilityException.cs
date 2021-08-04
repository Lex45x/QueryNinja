using System;

namespace QueryNinja.Core.Exceptions
{
    /// <summary>
    /// Exception is thrown in case of attempt to use behavior that is no loner supported. <br/>
    /// This type of exception has to be used for cases when compatibility change will not invoke compilation errors.
    /// </summary>
    public class CompatibilityException : QueryNinjaException
    {
        /// <inheritdoc />
        internal CompatibilityException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        private CompatibilityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}