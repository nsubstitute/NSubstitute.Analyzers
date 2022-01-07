using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.Refactorings;

internal static class AddModifierRefactoring
{
    public static Task<Document> RefactorAsync(Document document, SyntaxNode node, Accessibility accessibility)
    {
        SyntaxKind syntaxKind;

        switch (accessibility)
        {
            case Accessibility.Protected:
                syntaxKind = SyntaxKind.ProtectedKeyword;
                break;
            default:
                throw new NotSupportedException($"Adding {accessibility} modifier is not supported");
        }

        var newNode = Insert(node, syntaxKind);

        return document.ReplaceNodeAsync(node, newNode);
    }

    private static SyntaxNode Insert(SyntaxNode node, SyntaxKind syntaxKind)
    {
        switch (node)
        {
            case MethodStatementSyntax methodDeclarationSyntax:
                return methodDeclarationSyntax.WithModifiers(UpdateModifiers(methodDeclarationSyntax.Modifiers, syntaxKind));
            case PropertyStatementSyntax propertyDeclarationSyntax:
                return propertyDeclarationSyntax.WithModifiers(UpdateModifiers(propertyDeclarationSyntax.Modifiers, syntaxKind));
            default:
                throw new NotSupportedException($"Adding {syntaxKind} to {node.Kind()} is not supported");
        }
    }

    private static SyntaxTokenList UpdateModifiers(SyntaxTokenList modifiers, SyntaxKind modifier)
    {
        return modifiers.Any(modifier) ? modifiers : modifiers.Insert(0, Token(modifier));
    }
}