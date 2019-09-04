using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.VisualBasic.Extensions
{
    internal static class SyntaxExtensions
    {
        private static readonly int[] ParentInvocationKindHierarchy =
        {
            (int)SyntaxKind.SimpleMemberAccessExpression,
            (int)SyntaxKind.InvocationExpression
        };

        public static InvocationExpressionSyntax GetParentInvocationExpression(this SyntaxNode node)
        {
            return node.GetParentNode(ParentInvocationKindHierarchy) as InvocationExpressionSyntax;
        }

        public static SyntaxNode GetSubstitutionActualNode(this SyntaxNode node, Func<SyntaxNode, ISymbol> symbolProvider)
        {
            return node.GetSubstitutionActualNode<MemberAccessExpressionSyntax>(symbolProvider);
        }
    }
}