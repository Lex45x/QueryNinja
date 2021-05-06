using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace QueryNinja.Sources.GraphQL.Middleware
{
    internal interface IGraphQLRequestHandler
    {
        Task Handle(HttpContext context);
    }
}