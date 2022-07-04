using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.CSharp.Extensions;

internal static class SyntaxExtensions
{
    public static SyntaxNode GetSubstitutionActualNode(this SyntaxNode node, Func<SyntaxNode, ISymbol> symbolProvider)
    {
        return node.GetSubstitutionActualNode<MemberAccessExpressionSyntax>(symbolProvider);
    }
}