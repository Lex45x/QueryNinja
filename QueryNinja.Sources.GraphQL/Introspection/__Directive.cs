// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

using System.Collections.Generic;

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class __Directive
    {
        public __Directive(string name, IReadOnlyList<__DirectiveLocation> locations, IReadOnlyList<__InputValue> args,
            string description = null)
        {
            Name = name;
            Description = description;
            Locations = locations;
            Args = args;
        }

        public string Name { get; }
        public string Description { get; }
        public IReadOnlyList<__DirectiveLocation> Locations { get; }
        public IReadOnlyList<__InputValue> Args { get; }
    }
}