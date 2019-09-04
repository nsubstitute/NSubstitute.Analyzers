using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class SyntaxNodeExtensions
    {
        public static SyntaxNode GetParentNode(this SyntaxNode syntaxNode, IEnumerable<int> parentNodeHierarchyKinds)
        {
            using (var descendantNodesEnumerator = syntaxNode.DescendantNodes().GetEnumerator())
            {
                using (var hierarchyKindEnumerator = parentNodeHierarchyKinds.GetEnumerator())
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

        public static Location GetSubstitutionNodeActualLocation<TMemberAccessExpression>(this SyntaxNode node, ISymbol symbol)
            where TMemberAccessExpression : SyntaxNode
        {
            var actualNode = GetSubstitutionActualNode<TMemberAccessExpression>(node, symbol);
            return actualNode.GetLocation();
        }

        public static SyntaxNode GetSubstitutionActualNode<TMemberAccessExpression>(this SyntaxNode node, ISymbol symbol)
            where TMemberAccessExpression : SyntaxNode
        {
            return node.GetSubstitutionActualNode<TMemberAccessExpression>(syntax => symbol);
        }

        public static SyntaxNode GetSubstitutionActualNode<TMemberAccessExpression>(this SyntaxNode node, Func<SyntaxNode, ISymbol> symbolProvider)
            where TMemberAccessExpression : SyntaxNode
        {
            var actualNode = node is TMemberAccessExpression && symbolProvider(node) is IMethodSymbol _ ? node.Parent : node;

            return actualNode;
        }
    }
}