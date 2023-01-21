using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQLParser.AST;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using QueryNinja.Core;
using QueryNinja.Sources.GraphQL.Introspection;
using QueryNinja.Sources.GraphQL.SchemaGeneration;
using QueryNinja.Sources.GraphQL.Serializers;

namespace QueryNinja.Sources.GraphQL.Middleware
{
    internal class GraphQLQueriesSource : IGraphQLQueriesSource
    {
        private readonly Dictionary<string, IGraphQLQueryHandler> queryHandlers = new(StringComparer.OrdinalIgnoreCase);
        private readonly IActionsScanner scanner;
        private readonly IQuerySerializer<IDynamicQuery> serializer;

        public GraphQLQueriesSource(IActionsScanner scanner, IQuerySerializer<IDynamicQuery> serializer)
        {
            this.scanner = scanner;
            this.serializer = serializer;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IGraphQLQueryHandler> QueryHandlers
        {
            get
            {
                if (queryHandlers.Keys.Count == 0)
                {
                    BuildHandlers();
                }

                return queryHandlers;
            }
        }

        private void BuildHandlers()
        {
            var availableQueries = new List<__Field>();
            foreach (var queryRoot in scanner.GetQueryRoots())
            {
                var fieldType = __Type.FromType(queryRoot.ModelType);

                var queryName = queryRoot.ActionMethod.Name.Replace("Get", "");

                availableQueries.Add(new __Field(queryName, args: null, fieldType, isDeprecated: false,
                    deprecationReason: null));

            
                //todo: add IQuery support
                queryHandlers[queryName] = new ControllerQueryHandler(queryRoot, serializer);
            }

            var queryType = __Type.Object("Query", availableQueries);
            var schema = new __Schema(__Type.All, queryType);

            var introspectionQueryHandler = new IntrospectionQueryHandler(schema, serializer);

            queryHandlers["IntrospectionQuery"] = introspectionQueryHandler;
        }
    }

    internal class ControllerQueryHandler : IGraphQLQueryHandler
    {
        private readonly QueryRoot queryRoot;
        private readonly IQuerySerializer<IDynamicQuery> serializer;

        public ControllerQueryHandler(QueryRoot queryRoot, IQuerySerializer<IDynamicQuery> serializer)
        {
            this.queryRoot = queryRoot;
            this.serializer = serializer;
        }

        public Task<object> Handle(HttpContext context, GraphQLDocument document, JObject variables)
        {
            var query = serializer.Deserialize(document);
        }
    }
}