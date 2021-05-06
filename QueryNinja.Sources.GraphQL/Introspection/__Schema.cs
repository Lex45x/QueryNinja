// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

using System.Collections.Generic;

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class __Schema
    {
        public __Schema(IReadOnlySet<__Type> types, __Type queryType)
        {
            Types = types;
            QueryType = queryType;
        }

        public IReadOnlySet<__Type> Types { get; }
        public __Type QueryType { get; }
        public __Type MutationType { get; } = null;
        public __Type SubscriptionType { get; } = null;
        public IReadOnlyList<__Directive> Directives { get; } = new List<__Directive>();
    }
}