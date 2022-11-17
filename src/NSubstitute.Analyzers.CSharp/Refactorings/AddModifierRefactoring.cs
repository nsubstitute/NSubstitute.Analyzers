using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.Refactorings;

internal static class AddModifierRefactoring
{
    public static Task<Document> RefactorAsync(Document document, SyntaxNode node, Accessibility accessibility, CancellationToken cancellationToken)
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
            MethodDeclarationSyntax methodDeclarationSyntax => methodDeclarationSyntax.WithModifiers(
                UpdateModifiers(methodDeclarationSyntax.Modifiers, syntaxKind)),
            PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax.WithModifiers(
                UpdateModifiers(propertyDeclarationSyntax.Modifiers, syntaxKind)),
            IndexerDeclarationSyntax indexerDeclarationSyntax => indexerDeclarationSyntax.WithModifiers(
                UpdateModifiers(indexerDeclarationSyntax.Modifiers, syntaxKind)),
            _ => throw new NotSupportedException($"Adding {syntaxKind} to {node.Kind()} is not supported")
        };
    }

    private static SyntaxTokenList UpdateModifiers(SyntaxTokenList modifiers, SyntaxKind modifier)
    {
        return modifiers.Any(modifier) ? modifiers : modifiers.Insert(0, Token(modifier));
    }
}