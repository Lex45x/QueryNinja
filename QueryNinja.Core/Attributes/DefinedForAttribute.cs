using System;

namespace QueryNinja.Core.Attributes
{
    /// <summary>
    ///   Specify that usage of the <see cref="IQueryComponent" /> is defined on specific type and his descendants.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DefinedForAttribute : DefinitionAttribute
    {
        /// <summary>
        ///   Defines types that suitable for <see cref="IQueryComponent" />
        /// </summary>
        /// <param name="targetType"></param>
        public DefinedForAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        /// <summary>
        ///   Type suitable for <see cref="IQueryComponent" />
        /// </summary>
        public Type TargetType { get; }

        /// <inheritdoc />
        public override bool IsDefinedFor(Type type)
        {
            return TargetType.IsAssignableFrom(type);
        }
    }
}