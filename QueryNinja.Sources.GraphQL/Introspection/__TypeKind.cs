// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal enum __TypeKind
    {
        SCALAR = 0,
        OBJECT = 1,
        ENUM = 2,
        LIST = 3,
        NON_NULL = 4,
        //todo: types below are not supported
        INPUT_OBJECT = 5,
        INTERFACE = 6,
        UNION = 7
    }
}