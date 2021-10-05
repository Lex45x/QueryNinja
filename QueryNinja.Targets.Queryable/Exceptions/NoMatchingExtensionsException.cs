using System.Collections.Generic;
using QueryNinja.Core;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable.Exceptions
{
    /// <summary>
    ///   Occurred when <see cref="IQueryComponent" /> has no registered <see cref="IQueryBuilder" /> that
    ///   <see cref="IQueryBuilder.CanAppend" />.
    /// </summary>
    public class NoMatchingExtensionsException : QueryableTargetException
    {
        /// <inheritdoc />
        public NoMatchingExtensionsException(IQueryComponent component, IReadOnlyList<IQueryBuilder> currentBuilders)
            : base($"Query component '{component}' of type {component.GetType()} has no matching query builders.")
        {
            Component = component;
            CurrentBuilders = currentBuilders;
        }

        /// <summary>
        ///   Query component that caused exception.
        /// </summary>
        public IQueryComponent Component { get; }

        /// <summary>
        ///   Current collection of query builders.
        /// </summary>
        public IReadOnlyList<IQueryBuilder> CurrentBuilders { get; }
    }
}