using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeRefactoringProviders;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeRefactoringProviders;

[ExportCodeRefactoringProvider(LanguageNames.VisualBasic)]
internal sealed class IntroduceSubstituteCodeRefactoringProvider : AbstractIntroduceSubstituteCodeRefactoringProvider<ObjectCreationExpressionSyntax, ArgumentListSyntax, ArgumentSyntax>
{
    protected override IReadOnlyList<ArgumentSyntax> GetArgumentSyntaxNodes(ArgumentListSyntax argumentListSyntax, TextSpan span)
    {
        return argumentListSyntax.Arguments;
    }

    protected override ObjectCreationExpressionSyntax UpdateObjectCreationExpression(ObjectCreationExpressionSyntax objectCreationExpressionSyntax, IReadOnlyList<ArgumentSyntax> argumentSyntax)
    {
        var updatedArgumentList =
            objectCreationExpressionSyntax.ArgumentList.WithArguments(SeparatedList(argumentSyntax));

        return objectCreationExpressionSyntax.WithArgumentList(updatedArgumentList);
    }

    protected override bool IsMissing(ArgumentSyntax argumentSyntax)
    {
        return base.IsMissing(argumentSyntax) || argumentSyntax.IsOmitted;
    }

    protected override SyntaxNode FindSiblingNodeForLocalSubstitute(ObjectCreationExpressionSyntax creationExpression)
    {
        var container = creationExpression.Ancestors()
            .FirstOrDefault(ancestor => ancestor.Kind() == SyntaxKind.SubBlock);

        if (container is MethodBlockBaseSyntax methodBlockBaseSyntax)
        {
            return methodBlockBaseSyntax.Statements.FirstOrDefault();
        }

        return null;
    }

    protected override SyntaxNode FindSiblingNodeForReadonlySubstitute(SyntaxNode creationExpression)
    {
        var typeBlockSyntax = creationExpression.Ancestors()
            .OfType<TypeBlockSyntax>()
            .FirstOrDefault();

        return typeBlockSyntax?.Members.FirstOrDefault();
    }
}