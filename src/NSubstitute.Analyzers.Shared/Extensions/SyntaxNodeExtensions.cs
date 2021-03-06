using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class SyntaxNodeExtensions
    {
        public static SyntaxNode GetParentNode(this SyntaxNode syntaxNode, IEnumerable<int> parentNodeHierarchyKinds)
        {
            return GetNodeInHierarchy(syntaxNode.DescendantNodes(), parentNodeHierarchyKinds);
        }

        public static SyntaxNode GetAncestorNode(this SyntaxNode syntaxNode, IEnumerable<int> ancestorNodeHierarchyKinds)
        {
            return GetNodeInHierarchy(syntaxNode.Ancestors(), ancestorNodeHierarchyKinds);
        }

        public static SyntaxNode GetAncestorNode(this SyntaxNode syntaxNode, ImmutableArray<ImmutableArray<int>> ancestorHierarchies)
        {
            foreach (var possibleAncestorPath in ancestorHierarchies)
            {
                var node = syntaxNode.GetAncestorNode(possibleAncestorPath);

                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }

        public static Location GetSubstitutionNodeActualLocation<TMemberAccessExpression>(this SyntaxNode node, ISymbol symbol)
            where TMemberAccessExpression : SyntaxNode
        {
            var actualNode = node.GetSubstitutionActualNode<TMemberAccessExpression>(syntax => symbol);

            return actualNode.GetLocation();
        }

        public static SyntaxNode GetSubstitutionActualNode<TMemberAccessExpression>(this SyntaxNode node, Func<SyntaxNode, ISymbol> symbolProvider)
            where TMemberAccessExpression : SyntaxNode
        {
            var actualNode = node is TMemberAccessExpression && symbolProvider(node) is IMethodSymbol _ ? node.Parent : node;

            return actualNode;
        }

        private static SyntaxNode GetNodeInHierarchy(IEnumerable<SyntaxNode> nodes, IEnumerable<int> hierarchyKindPath)
        {
            using (var descendantNodesEnumerator = nodes.GetEnumerator())
            {
                using (var hierarchyKindEnumerator = hierarchyKindPath.GetEnumerator())
                {
                    while (hierarchyKindEnumerator.MoveNext() && descendantNodesEnumerator.MoveNext())
                    {
                        if (descendantNodesEnumerator.Current.RawKind != hierarchyKindEnumerator.Current)
                        {
                            return null;
                        }
                    }

                    if (hierarchyKindEnumerator.MoveNext() == false)
                    {
                        return descendantNodesEnumerator.Current;
                    }
                }
            }

            return null;
        }
    }
}