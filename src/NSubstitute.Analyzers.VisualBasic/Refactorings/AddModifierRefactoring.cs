using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.Refactorings;

internal static class AddModifierRefactoring
{
    public static Task<Document> RefactorAsync(
        Document document,
        SyntaxNode node,
        Accessibility accessibility,
        CancellationToken cancellationToken)
    {
        var syntaxKind = accessibility switch
        {
            Accessibility.Protected => SyntaxKind.ProtectedKeyword,
            _ => throw new NotSupportedException($"Adding {accessibility} modifier is not supported")
        };

        var newNode = Insert(node, syntaxKind);

        return document.ReplaceNodeAsync(node, newNode, cancellationToken: cancellationToken);
    }

    private static SyntaxNode Insert(SyntaxNode node, SyntaxKind syntaxKind)
    {
        return node switch
        {
            MethodStatementSyntax methodDeclarationSyntax => methodDeclarationSyntax.WithModifiers(
                UpdateModifiers(methodDeclarationSyntax.Modifiers, syntaxKind)),
            PropertyStatementSyntax propertyDeclarationSyntax => propertyDeclarationSyntax.WithModifiers(
                UpdateModifiers(propertyDeclarationSyntax.Modifiers, syntaxKind)),
            _ => throw new NotSupportedException($"Adding {syntaxKind} to {node.Kind()} is not supported")
        };
    }

    private static SyntaxTokenList UpdateModifiers(SyntaxTokenList modifiers, SyntaxKind modifier)
    {
        return modifiers.Any(modifier) ? modifiers : modifiers.Insert(0, Token(modifier));
    }
}