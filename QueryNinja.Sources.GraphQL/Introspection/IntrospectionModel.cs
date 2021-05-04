// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

using System;
using System.Linq;

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class IntrospectionModel
    {
        public __Schema __Schema { get; set; }

        public __Type __Type(string name)
        {
            return __Schema.Types.FirstOrDefault(type => type.Name == name);
        }
    }
}