using System;
using System.Collections.Generic;
using System.Linq;
using QueryNinja.Core;
using QueryNinja.Sources.GraphQL.Introspection;
using QueryNinja.Sources.GraphQL.SchemaGeneration;
using QueryNinja.Sources.GraphQL.Serializers;

namespace QueryNinja.Sources.GraphQL.Middleware
{
    internal class GraphQLQueriesSource : IGraphQLQueriesSource
    {
        private readonly IActionsScanner scanner;
        private readonly IQuerySerializer<IDynamicQuery> serializer;
        private readonly Dictionary<string, IGraphQLQueryHandler> queryHandlers = new(StringComparer.OrdinalIgnoreCase);

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
                //todo: specific query handlers should be created here.
                var includeFields = queryRoot.QueryType.IsAssignableTo(typeof(IDynamicQuery));

                var fieldType = __Type.FromType(includeFields ? queryRoot.ModelType : typeof(object));

                var queryName = queryRoot.ActionMethod.Name.Replace("Get", "");
                
                //todo: provide proper types for arguments
                //var arguments = queryRoot.ActionMethod.GetParameters()
                //    .Select(info => new __InputValue(info.Name, __Type.FromType(info.ParameterType), info.DefaultValue?.ToString()))
                //    .ToList();

                availableQueries.Add(new __Field(queryName, null, fieldType, false, null));
            }

            var queryType = __Type.Object("Query", availableQueries);
            var schema = new __Schema(__Type.All, queryType);
            
            var introspectionQueryHandler = new IntrospectionQueryHandler(schema, serializer);

            queryHandlers["IntrospectionQuery"] = introspectionQueryHandler;
        }
    }
}