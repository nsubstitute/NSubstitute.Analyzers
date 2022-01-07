using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class SubstituteAnalyzer : AbstractSubstituteAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax, ArgumentSyntax>
{
    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

    public SubstituteAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, SubstituteProxyAnalysis.Instance, SubstituteConstructorAnalysis.Instance, SubstituteConstructorMatcher.Instance)
    {
    }

    protected override InvocationExpressionSyntax GetCorrespondingSubstituteInvocationExpressionSyntax(InvocationExpressionSyntax invocationExpressionSyntax, string substituteName)
    {
        var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)invocationExpressionSyntax.Expression;

        return invocationExpressionSyntax.WithExpression(
            memberAccessExpressionSyntax.WithName(
                memberAccessExpressionSyntax.Name.WithIdentifier(Identifier(substituteName))));
    }

    protected override InvocationExpressionSyntax GetSubstituteInvocationExpressionSyntaxWithoutConstructorArguments(InvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol)
    {
        ArgumentListSyntax argumentListSyntax;
        if (methodSymbol.IsGenericMethod)
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