using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class SyntaxNodeExtensions
{
    public static SyntaxNode GetParentNode(this SyntaxNode syntaxNode, IEnumerable<int> parentNodeHierarchyKinds)
    {
        return GetNodeInHierarchy(syntaxNode.DescendantNodes(), parentNodeHierarchyKinds);
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