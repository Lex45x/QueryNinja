// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class __EnumValue
    {
        public __EnumValue(string name, bool isDeprecated, string deprecationReason, string description = null)
        {
            Name = name;
            Description = description;
            IsDeprecated = isDeprecated;
            DeprecationReason = deprecationReason;
        }

        public string Name { get; }
        public string Description { get; }
        public bool IsDeprecated { get; }
        public string DeprecationReason { get; }
    }
}