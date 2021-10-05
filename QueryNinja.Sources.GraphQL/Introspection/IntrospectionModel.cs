// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

using System.Linq;

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class IntrospectionModel
    {
        public IntrospectionModel(__Schema schema)
        {
            __Schema = schema;
        }

        public __Schema __Schema { get; }

        public __Type __Type(string name)
        {
            return __Schema.Types.FirstOrDefault(type => type.Name == name);
        }
    }
}