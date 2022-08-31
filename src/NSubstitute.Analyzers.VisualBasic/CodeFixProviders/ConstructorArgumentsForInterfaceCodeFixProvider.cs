using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.VisualBasic)]
internal sealed class ConstructorArgumentsForInterfaceCodeFixProvider : AbstractConstructorArgumentsForInterfaceCodeFixProvider
{
    protected override SyntaxNode GetInvocationExpressionSyntaxWithEmptyArgumentList(IInvocationOperation invocationOperation)
    {
        var invocationExpression = (InvocationExpressionSyntax)invocationOperation.Syntax;

        return invocationExpression.WithArgumentList(ArgumentList());
    }

    protected override SyntaxNode GetInvocationExpressionSyntaxWithNullConstructorArgument(IInvocationOperation invocationOperation)
    {
        var invocationExpressionSyntax = (InvocationExpressionSyntax)invocationOperation.Syntax;
        var arguments = invocationOperation.Arguments
            .OrderBy(x => x.Syntax.GetLocation().GetLineSpan().StartLinePosition.Character)
            .Select<IArgumentOperation, ArgumentSyntax>(argumentOperation =>
            {
                var argumentSyntax = (SimpleArgumentSyntax)argumentOperation.Syntax;
                if (argumentOperation.Parameter.Ordinal > 0)
                {
                    argumentSyntax = argumentSyntax.WithExpression(
                        LiteralExpression(SyntaxKind.NothingLiteralExpression, Token(SyntaxKind.NothingKeyword)));
                }

                return argumentSyntax;
            });

        return invocationExpressionSyntax.WithArgumentList(ArgumentList(SeparatedList(arguments)));
    }
}