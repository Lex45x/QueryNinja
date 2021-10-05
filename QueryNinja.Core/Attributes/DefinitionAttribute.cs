using System;

namespace QueryNinja.Core.Attributes
{
    /// <summary>
    ///   Base class for all Attributes that can describe defined types for <see cref="IQueryComponent" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public abstract class DefinitionAttribute : Attribute
    {
        /// <summary>
        ///   Allows to check whether <paramref name="type" /> is defined for <see cref="IQueryComponent" />.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract bool IsDefinedFor(Type type);
    }
}