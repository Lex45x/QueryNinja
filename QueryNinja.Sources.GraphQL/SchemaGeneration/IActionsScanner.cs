using System.Collections.Generic;

namespace QueryNinja.Sources.GraphQL.SchemaGeneration
{
    /// <summary>
    /// Allows to get all <see cref="QueryRoot"/>s from Asp.Net Core application.
    /// </summary>
    public interface IActionsScanner
    {
        /// <summary>
        /// Scans all action methods and returns all found <see cref="QueryRoot"/>s.
        /// </summary>
        /// <returns></returns>
        IEnumerable<QueryRoot> GetQueryRoots();
    }
}