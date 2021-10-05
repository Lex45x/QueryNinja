// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

using System.Collections.Generic;

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class __Field
    {
        public __Field(string name, IReadOnlyList<__InputValue> args, __Type type, bool isDeprecated,
            string deprecationReason, string description = null)
        {
            Name = name;
            Args = args ?? new List<__InputValue>();
            Type = type;
            IsDeprecated = isDeprecated;
            DeprecationReason = deprecationReason;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
        public IReadOnlyList<__InputValue> Args { get; }
        public __Type Type { get; }
        public bool IsDeprecated { get; }
        public string DeprecationReason { get; }
    }
}