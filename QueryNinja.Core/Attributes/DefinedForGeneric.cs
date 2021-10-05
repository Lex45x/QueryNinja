﻿using System;

namespace QueryNinja.Core.Attributes
{
    /// <summary>
    ///   Specify that usage of the <see cref="IQueryComponent" /> is defined on constructed generic types. <br />
    ///   Will compare <see cref="GenericTypeDefinition" /> with Definition of Type, Definition of implemented interfaces and
    ///   definition of BaseType.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DefinedForGeneric : DefinitionAttribute
    {
        /// <summary>
        ///   Defines Generic Type Definition that suitable for <see cref="IQueryComponent" />
        /// </summary>
        /// <param name="genericTypeDefinition"></param>
        public DefinedForGeneric(Type genericTypeDefinition)
        {
            if (!genericTypeDefinition.IsGenericTypeDefinition)
            {
                throw new ArgumentException("The type should be GenericTypeDefinition", nameof(genericTypeDefinition));
            }

            GenericTypeDefinition = genericTypeDefinition;
        }

        /// <summary>
        ///   Type suitable for <see cref="IQueryComponent" />
        /// </summary>
        public Type GenericTypeDefinition { get; }

        /// <inheritdoc />
        public override bool IsDefinedFor(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == GenericTypeDefinition
                                          || type.GetInterface(GenericTypeDefinition.Name).GetGenericTypeDefinition() ==
                                          GenericTypeDefinition
                                          || type.BaseType?.GetGenericTypeDefinition() == GenericTypeDefinition);
        }
    }
}