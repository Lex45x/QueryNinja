using Newtonsoft.Json.Linq;

namespace QueryNinja.Sources.GraphQL.Middleware
{
    internal class GraphQLRequest
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }
    }
}