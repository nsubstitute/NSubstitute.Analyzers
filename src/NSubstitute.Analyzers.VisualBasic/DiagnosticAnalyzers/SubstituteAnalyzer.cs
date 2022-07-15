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
            var nullSyntax = SimpleArgument(LiteralExpression(SyntaxKind.NothingLiteralExpression, Token(SyntaxKind.NothingKeyword)));
            argumentListSyntax = ArgumentList(SeparatedList(invocationExpressionSyntax.ArgumentList.Arguments.Take(1)).Add(nullSyntax));
        }

        return invocationExpressionSyntax.WithArgumentList(argumentListSyntax);
    }
}