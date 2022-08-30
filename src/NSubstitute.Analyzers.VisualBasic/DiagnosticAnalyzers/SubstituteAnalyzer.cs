using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class SubstituteAnalyzer : AbstractSubstituteAnalyzer
{
    public SubstituteAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, SubstituteProxyAnalysis.Instance, SubstituteConstructorAnalysis.Instance, SubstituteConstructorMatcher.Instance)
    {
    }

    protected override SyntaxNode GetCorrespondingSubstituteInvocationExpressionSyntax(IInvocationOperation invocationOperation, string substituteName)
    {
        var invocationExpressionSyntax = (InvocationExpressionSyntax)invocationOperation.Syntax;
        var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)invocationExpressionSyntax.Expression;

        return invocationExpressionSyntax.WithExpression(
            memberAccessExpressionSyntax.WithName(
                memberAccessExpressionSyntax.Name.WithIdentifier(Identifier(substituteName))));
    }

    protected override SyntaxNode GetSubstituteInvocationExpressionSyntaxWithoutConstructorArguments(IInvocationOperation invocationOperation)
    {
        var invocationExpressionSyntax = (InvocationExpressionSyntax)invocationOperation.Syntax;
        ArgumentListSyntax argumentListSyntax;
        if (invocationOperation.TargetMethod.IsGenericMethod)
        {
            argumentListSyntax = ArgumentList();
        }
        else
        {
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

            argumentListSyntax = ArgumentList(SeparatedList(arguments));
        }

        return invocationExpressionSyntax.WithArgumentList(argumentListSyntax);
    }
}