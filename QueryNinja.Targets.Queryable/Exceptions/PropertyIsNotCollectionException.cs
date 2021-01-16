using System;
using System.Collections.Generic;

namespace QueryNinja.Targets.Queryable.Exceptions
{
    /// <summary>
    /// Property <see cref="Property"/> is expected to implement <see cref="IEnumerable{T}"/>.
    /// </summary>
    public class PropertyIsNotCollectionException : QueryableTargetException
    {
        /// <summary>
        /// Property of wrong type.
        /// </summary>
        public string Property { get; }

        /// <summary>
        /// Actual type of the property.
        /// </summary>
        public Type PropertyType { get; }

        /// <summary>
        /// Type of the entity.
        /// </summary>
        public Type EntityType { get; }

        /// <inheritdoc />
        public PropertyIsNotCollectionException(string property, Type propertyType, Type entityType)
            : base(
                $"Property {property} of type {propertyType} in entity {entityType} is expected to implement generic IEnumerable interface to apply collection filter")
        {
            Property = property;
            PropertyType = propertyType;
            EntityType = entityType;
        }
    }
}