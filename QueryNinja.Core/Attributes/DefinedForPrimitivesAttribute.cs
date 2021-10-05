using System;

namespace QueryNinja.Core.Attributes
{
    /// <summary>
    ///   Specify that usage of the <see cref="IQueryComponent" /> is defined on Primitive types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DefinedForPrimitivesAttribute : DefinitionAttribute
    {
        /// <inheritdoc />
        public override bool IsDefinedFor(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type.IsEnum;
        }
    }
}