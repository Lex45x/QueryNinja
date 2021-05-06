using System.Threading.Tasks;
using GraphQLParser.AST;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace QueryNinja.Sources.GraphQL.Middleware
{
    /// <summary>
    /// Contains logic of handling specific GraphQL Query Operation
    /// </summary>
    public interface IGraphQLQueryHandler
    {
        /// <summary>
        /// Handles GraphQL Query Operation based on <see cref="HttpContext"/>, <see cref="GraphQLDocument"/> and additional GraphQL variables.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="document"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        Task<object> Handle(HttpContext context, GraphQLDocument document, JObject variables);
    }
}