using GraphQLParser.AST;
using QueryNinja.Core;

namespace QueryNinja.Sources.GraphQL.Serializers
{
    /// <summary>
    ///   Allows to serialize specific <see cref="IQuery" /> instance from <see cref="GraphQLDocument" />
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    public interface IQuerySerializer<out TQuery>
        where TQuery : IQuery
    {
        /// <summary>
        ///   Creates instance of <typeparamref name="TQuery" /> based on <paramref name="document" />
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        TQuery Deserialize(GraphQLDocument document);
    }
}