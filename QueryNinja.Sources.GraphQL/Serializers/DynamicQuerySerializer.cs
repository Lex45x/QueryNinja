using System;
using System.Collections.Generic;
using System.Linq;
using GraphQLParser.AST;
using QueryNinja.Core;
using QueryNinja.Core.Projection;

namespace QueryNinja.Sources.GraphQL.Serializers
{
    internal class DynamicQuerySerializer : IQuerySerializer<IDynamicQuery>
    {
        /// <inheritdoc />
        public IDynamicQuery Deserialize(GraphQLDocument document)
        {
            var fragments = document.Definitions!
                .OfType<GraphQLFragmentDefinition>()
                .ToDictionary(fragment => fragment.Name!.Value.ToString());

            var operation = document.Definitions
                ?.OfType<GraphQLOperationDefinition>()
                .Single();

            var selectors =
                ProcessSelectionSet(operation.SelectionSet.Selections, name => fragments[name.Value.ToString()])
                    .ToList();

            //todo: implement deserialization here
            return new DynamicQuery(new List<IQueryComponent>(), selectors);
        }

        private static IEnumerable<GraphQLFieldSelection> ResolveFragments(IEnumerable<ASTNode> nodes,
            Func<GraphQLName, GraphQLFragmentDefinition> fragmentResolver)
        {
            if (nodes == null)
            {
                yield break;
            }

            foreach (var node in nodes)
            {
                switch (node)
                {
                    case GraphQLFieldSelection selection:
                        yield return selection;
                        break;
                    case GraphQLFragmentSpread fragment:
                    {
                        var fragmentSelection = fragmentResolver(fragment.Name).SelectionSet?.Selections;

                        var resolvedSelections = ResolveFragments(fragmentSelection, fragmentResolver);

                        foreach (var resolvedSelection in resolvedSelections)
                        {
                            yield return resolvedSelection;
                        }

                        break;
                    }
                }
            }
        }

        private static IEnumerable<ISelector> ProcessSelectionSet(IEnumerable<ASTNode> selections,
            Func<GraphQLName, GraphQLFragmentDefinition> fragmentResolver)
        {
            foreach (var selection in ResolveFragments(selections, fragmentResolver))
            {
                var arguments =
                    selection.Arguments?.ToDictionary(
                        argument => argument.Name?.Value.ToString(),
                        argument => argument.Value?.ToString());

                var nestedSelectors = ProcessSelectionSet(selection.SelectionSet?.Selections, fragmentResolver)
                    .ToList();

                yield return new Selector(selection.Name!.Value.ToString(), arguments, nestedSelectors);
            }
        }
    }
}