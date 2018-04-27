using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Extensions
{
    public static class SyntaxNodeExtensions
    {
        public static IEnumerable<SyntaxNode> DescendantNodesDeep(this SyntaxNode node)
        {
            var stack = new Stack<SyntaxNode>();
            stack.Push(node);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in next.DescendantNodes())
                {
                    stack.Push(child);
                }
            }
        }
    }
}