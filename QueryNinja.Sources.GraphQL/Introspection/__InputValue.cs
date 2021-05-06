// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class __InputValue
    {
        public __InputValue(string name, __Type type, string defaultValue, string description = null)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
        public __Type Type { get; }
        public string DefaultValue { get; }
    }
}