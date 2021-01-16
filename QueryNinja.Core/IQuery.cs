using System.Collections.Generic;

namespace QueryNinja.Core
{
    /// <summary>
    ///   This is core interface for all types of queries
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        ///   Returns a single collections with all query components.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IQueryComponent> GetComponents();
    }
}