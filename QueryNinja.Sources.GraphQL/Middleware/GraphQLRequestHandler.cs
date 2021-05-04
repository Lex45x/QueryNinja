using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQLParser;
using GraphQLParser.AST;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using QueryNinja.Sources.GraphQL.SchemaGeneration;

namespace QueryNinja.Sources.GraphQL.Middleware
{
    internal interface IGraphQLRequestHandler
    {
        Task Handle(HttpContext context);
    }


    internal class GraphQLRequest
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }
    }

    internal interface IGraphQLQueriesSource
    {
        IReadOnlyDictionary<string, IGraphQLQueryHandler> QueryHandlers { get; }
    }

    internal class GraphQLQueriesSource : IGraphQLQueriesSource
    {
        private readonly IActionsScanner scanner;
        private readonly IReadOnlyDictionary<string, IGraphQLQueryHandler> queryHandlers;

        public GraphQLQueriesSource(IActionsScanner scanner)
        {
            this.scanner = scanner;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IGraphQLQueryHandler> QueryHandlers
        {
            get
            {
                if (queryHandlers == null)
                {
                    BuildHandlers();
                }

                return queryHandlers;
            }
        }

        private void BuildHandlers()
        {
            foreach (var queryRoot in scanner.GetQueryRoots())
            {
                throw new NotImplementedException();
            }
        }
    }

    internal sealed class GraphQLRequestHandler : IGraphQLRequestHandler
    {
        private readonly IGraphQLQueriesSource queriesSource;

        public GraphQLRequestHandler(IGraphQLQueriesSource queriesSource)
        {
            this.queriesSource = queriesSource;
        }
        public async Task Handle(HttpContext context)
        {
            var request = await context.Request.ReadFromJsonAsync<GraphQLRequest>();

            if (request == null)
            {
                throw new InvalidOperationException("Unsupported request body");
            }

            using var graphQLDocument = Parser.Parse(request.Query);
            
            var operationName = GetOperationName(request, graphQLDocument);

            if (queriesSource.QueryHandlers.TryGetValue(operationName, out var handler))
            {
                await handler.Handle(context, graphQLDocument, request.Variables);
            }
            else
            {
                throw new InvalidOperationException("Invalid operation name");
            }
        }

        private static string GetOperationName(GraphQLRequest request, GraphQLDocument graphQLDocument)
        {
            string operationName;

            if (!string.IsNullOrWhiteSpace(request.OperationName))
            {
                operationName = request.OperationName;
            }
            else
            {
                operationName = graphQLDocument.Definitions?
                    .OfType<GraphQLOperationDefinition>()
                    .SingleOrDefault(node => node.Operation == OperationType.Query)
                    ?.Name?.ToString();
            }

            if (operationName == null)
            {
                throw new InvalidOperationException("Operation name is not found");
            }

            return operationName;
        }
    }

    internal interface IGraphQLQueryHandler
    {
        Task Handle(HttpContext context, GraphQLDocument document, JObject variables);
    }
}