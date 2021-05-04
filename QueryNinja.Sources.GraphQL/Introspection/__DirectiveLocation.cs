// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal enum __DirectiveLocation
    {
        QUERY = 0,
        MUTATION = 1,
        SUBSCRIPTION = 2,
        FIELD = 3,
        FRAGMENT_DEFINITION = 4,
        FRAGMENT_SPREAD = 5,
        INLINE_FRAGMENT = 6,
        SCHEMA = 7,
        SCALAR = 8,
        OBJECT = 9,
        FIELD_DEFINITION = 10,
        ARGUMENT_DEFINITION = 11,
        INTERFACE = 12,
        UNION = 13,
        ENUM = 14,
        ENUM_VALUE = 15,
        INPUT_OBJECT = 16,
        INPUT_FIELD_DEFINITION = 17
    }
}