using System.Collections.Generic;

namespace QueryNinja.Sources.GraphQL.Middleware
{
    internal interface IGraphQLQueriesSource
    {
        IReadOnlyDictionary<string, IGraphQLQueryHandler> QueryHandlers { get; }
    }
}