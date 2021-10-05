using QueryNinja.Core;
using QueryNinja.Core.Extensibility;

namespace QueryNinja.Sources.GraphQL
{
    /// <summary>
    ///   Allows schema generation and creating instances of <see cref="IQueryComponent" />
    /// </summary>
    public interface IQueryComponentDescriptor : IQueryComponentExtension
    {
    }
}