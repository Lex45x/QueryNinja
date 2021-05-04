// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class __InputValue
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public __Type Type { get; set; }
        public string DefaultValue { get; set; }
    }
}