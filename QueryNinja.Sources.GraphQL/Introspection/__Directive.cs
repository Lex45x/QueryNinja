﻿// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

using System.Collections.Generic;

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class __Directive
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<__DirectiveLocation> Locations { get; set; }
        public IReadOnlyList<__InputValue> Args { get; set; }
    }
}