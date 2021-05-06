using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQLParser;
using GraphQLParser.AST;
using Microsoft.AspNetCore.Http;

namespace QueryNinja.Sources.GraphQL.Middleware
{
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
                var response = await handler.Handle(context, graphQLDocument, request.Variables);

                await context.Response.WriteAsJsonAsync(new
                {
                    Data = response
                });
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
}