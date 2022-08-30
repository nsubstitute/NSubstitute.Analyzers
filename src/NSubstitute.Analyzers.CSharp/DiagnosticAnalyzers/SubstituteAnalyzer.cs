using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class SubstituteAnalyzer : AbstractSubstituteAnalyzer
{
    public SubstituteAnalyzer()
        : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance, SubstituteProxyAnalysis.Instance, SubstituteConstructorAnalysis.Instance, SubstituteConstructorMatcher.Instance)
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
            var arguments = invocationOperation.Arguments.Select(argumentOperation =>
            {
                var argumentSyntax = (ArgumentSyntax)argumentOperation.Syntax;
                if (argumentOperation.Parameter.Ordinal > 0)
                {
                    argumentSyntax = argumentSyntax.WithExpression(LiteralExpression(SyntaxKind.NullLiteralExpression));
                }

                return argumentSyntax;
            });

            argumentListSyntax = ArgumentList(SeparatedList(arguments));
        }

        return invocationExpressionSyntax.WithArgumentList(argumentListSyntax);
    }
}