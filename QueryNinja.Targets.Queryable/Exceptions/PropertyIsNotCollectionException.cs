using System;
using System.Collections.Generic;
using QueryNinja.Core.Exceptions;

namespace QueryNinja.Targets.Queryable.Exceptions
{
    /// <summary>
    /// Property <see cref="Property"/> is expected to implement <see cref="IEnumerable{T}"/>.
    /// </summary>
    public class PropertyIsNotCollectionException : QueryableTargetException
    {
        public string Property { get; }
        public Type EntityType { get; }

        public PropertyIsNotCollectionException(string property, Type entityType)
            : base($"Property {property} in entity {entityType} is expected to implement generic IEnumerable interface to apply collection filter")
        {
            Property = property;
            EntityType = entityType;
        }
    }
}