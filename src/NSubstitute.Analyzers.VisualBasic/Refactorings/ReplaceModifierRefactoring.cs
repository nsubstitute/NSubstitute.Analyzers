using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.Refactorings;

internal class ReplaceModifierRefactoring
{
    public static Task<Document> RefactorAsync(Document document, SyntaxNode node, Accessibility fromAccessibility, Accessibility toAccessibility)
    {
        var fromSyntaxKind = InferSyntaxKind(fromAccessibility);
        var toSyntaxKind = InferSyntaxKind(toAccessibility);

        var newNode = ReplaceModifier(node, fromSyntaxKind, toSyntaxKind);

        return document.ReplaceNodeAsync(node, newNode);
    }

    private static SyntaxKind InferSyntaxKind(Accessibility fromAccessibility)
    {
        SyntaxKind syntaxKind;
        switch (fromAccessibility)
        {
            case Accessibility.Internal:
                syntaxKind = SyntaxKind.FriendKeyword;
                break;
            case Accessibility.Public:
                syntaxKind = SyntaxKind.PublicKeyword;
                break;
            default:
                throw new NotSupportedException($"Replacing {fromAccessibility} modifier is not supported");
        }

        return syntaxKind;
    }

    private static SyntaxNode ReplaceModifier(SyntaxNode node, SyntaxKind fromSyntaxKind, SyntaxKind toSyntaxKind)
    {
        switch (node)
        {
            case MethodStatementSyntax methodDeclarationSyntax:
                return methodDeclarationSyntax.WithModifiers(ReplaceModifier(methodDeclarationSyntax.Modifiers, fromSyntaxKind, toSyntaxKind));
            case PropertyStatementSyntax propertyDeclarationSyntax:
                return propertyDeclarationSyntax.WithModifiers(ReplaceModifier(propertyDeclarationSyntax.Modifiers, fromSyntaxKind, toSyntaxKind));
            default:
                throw new NotSupportedException($"Replacing {fromSyntaxKind} in {node.Kind()} is not supported");
        }
    }

    private static SyntaxTokenList ReplaceModifier(SyntaxTokenList modifiers, SyntaxKind fromModifier, SyntaxKind toModifier)
    {
        var modifier = modifiers.First(mod => mod.IsKind(fromModifier));
        return modifiers.Replace(modifier, Token(toModifier));
    }
}