using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NSubstitute.Analyzers.Shared.CodeRefactoringProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeRefactoringProviders;

[ExportCodeRefactoringProvider(LanguageNames.CSharp)]
internal sealed class IntroduceSubstituteCodeRefactoringProvider : AbstractIntroduceSubstituteCodeRefactoringProvider<ObjectCreationExpressionSyntax, ArgumentListSyntax, ArgumentSyntax>
{
    protected override IReadOnlyList<ArgumentSyntax> GetArgumentSyntaxNodes(ArgumentListSyntax argumentListSyntax, TextSpan span)
    {
        return argumentListSyntax.Arguments;
    }

    protected override ObjectCreationExpressionSyntax UpdateObjectCreationExpression(
        ObjectCreationExpressionSyntax objectCreationExpressionSyntax,
        IReadOnlyList<ArgumentSyntax> updatedArguments)
    {
        var originalArgumentList = objectCreationExpressionSyntax.ArgumentList ?? ArgumentList();
        var updatedArgumentList = originalArgumentList.Update(
            originalArgumentList.OpenParenToken.WithTrailingTrivia(),
            SeparatedList(updatedArguments),
            originalArgumentList.CloseParenToken);

        updatedArgumentList = UpdateArgumentListTrivia(originalArgumentList, updatedArgumentList);

        return objectCreationExpressionSyntax.WithArgumentList(updatedArgumentList);
    }

    protected override SyntaxNode? FindSiblingNodeForLocalSubstitute(ObjectCreationExpressionSyntax creationExpression)
    {
        var container = creationExpression.Ancestors()
            .FirstOrDefault(ancestor => ancestor.Kind() == SyntaxKind.Block);

        return container?.ChildNodes().FirstOrDefault();
    }

    protected override SyntaxNode? FindSiblingNodeForReadonlySubstitute(SyntaxNode creationExpression)
    {
        var typeDeclarationSyntax = creationExpression.Ancestors()
            .OfType<TypeDeclarationSyntax>()
            .FirstOrDefault();

        return typeDeclarationSyntax?.Members.FirstOrDefault();
    }

    private static ArgumentListSyntax UpdateArgumentListTrivia(
        ArgumentListSyntax originalArgumentList,
        ArgumentListSyntax updatedArgumentList)
    {
        if (originalArgumentList.CloseParenToken.IsMissing == false)
        {
            return updatedArgumentList;
        }

        var token = originalArgumentList.ChildTokens().LastOrDefault(innerToken => innerToken.IsMissing == false);

        if (token.Kind() != SyntaxKind.None && token.HasTrailingTrivia)
        {
            updatedArgumentList = updatedArgumentList.WithTrailingTrivia(token.TrailingTrivia);
        }

        return updatedArgumentList;
    }
}