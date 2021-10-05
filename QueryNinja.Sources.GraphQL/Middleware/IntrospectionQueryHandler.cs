using System.Linq;
using System.Threading.Tasks;
using GraphQLParser.AST;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using QueryNinja.Core;
using QueryNinja.Sources.GraphQL.Introspection;
using QueryNinja.Sources.GraphQL.Serializers;
using QueryNinja.Targets.Queryable;

namespace QueryNinja.Sources.GraphQL.Middleware
{
    internal class IntrospectionQueryHandler : IGraphQLQueryHandler
    {
        private readonly IQueryable<IntrospectionModel> introspectionSource;
        private readonly IQuerySerializer<IDynamicQuery> serializer;

        public IntrospectionQueryHandler(__Schema schema, IQuerySerializer<IDynamicQuery> serializer)
        {
            this.serializer = serializer;
            introspectionSource = Enumerable.Repeat(new IntrospectionModel(schema), count: 1).AsQueryable();
        }

        /// <inheritdoc />
        public Task<object> Handle(HttpContext context, GraphQLDocument document, JObject variables)
        {
            var query = serializer.Deserialize(document);

            //source should tolerate selects from null
            var queryable = introspectionSource.WithQuery(query);

            var visitor = new NullableFriendlyVisitor();

            var resultQueryable = queryable.Provider.CreateQuery<object>(visitor.Visit(queryable.Expression));

            var result = resultQueryable.FirstOrDefault();

            return Task.FromResult(result);
        }
    }
}