using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;

namespace QueryNinja.Sources.AspNetCore
{
    /// <summary>
    /// Allows to create <see cref="IFilter"/> instance from Query Parameters
    /// </summary>
    public interface IQueryComponentFactory : IQueryComponentExtension
    {
        /// <summary>
        /// Checks whether current factory can be applied to the specified query parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CanApply(string name, string value);

        /// <summary>
        /// Creates <see cref="IFilter"/> instance from the specified query parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IQueryComponent Create(string name, string value);
    }
}