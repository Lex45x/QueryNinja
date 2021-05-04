// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

using System.Collections.Generic;

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class __Schema
    {
        public IReadOnlyList<__Type> Types { get; set; }
        public __Type QueryType { get; set; }
        public __Type MutationType { get; set; }
        public __Type SubscriptionType { get; set; }
        public IReadOnlyList<__Directive> Directives { get; set; }
    }
}