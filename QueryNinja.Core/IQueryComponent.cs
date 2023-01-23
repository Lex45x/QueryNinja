using System;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Core
{
    /// <summary>
    /// Represent a generic query component such as <see cref="IFilter"/> or <see cref="OrderingRule"/>
    /// todo: current abstractions seems to add significant amount of restrictions.
    /// </summary>
    public interface IQueryComponent : IEquatable<IQueryComponent>
    {

    }
}