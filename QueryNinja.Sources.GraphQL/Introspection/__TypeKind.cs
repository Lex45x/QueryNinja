// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal enum __TypeKind
    {
        SCALAR = 0,
        OBJECT = 1,
        INTERFACE = 2,
        UNION = 3,
        ENUM = 4,
        INPUT_OBJECT = 5,
        LIST = 6,
        NON_NULL = 7
    }
}