using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using QueryNinja.Sources.GraphQL.Middleware;

namespace QueryNinja.Sources.GraphQL
{
    /// <summary>
    ///   Contains extensions needed to setup GraphQL endpoint.
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapGraphQL(this IEndpointRouteBuilder builder, string route = "/graphql")
        {
            //need to map get to support usage of Altair client
            //this will add extra CI steps with NPM and static files for the package
            builder.MapPost(route, GraphQLEndpointDelegate);
            return builder;
        }

        private static Task GraphQLEndpointDelegate(HttpContext context)
        {
            var graphQLRequestHandler = context.RequestServices.GetRequiredService<IGraphQLRequestHandler>();

            return graphQLRequestHandler.Handle(context);
        }
    }
}