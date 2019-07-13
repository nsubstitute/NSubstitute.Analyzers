using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class SubstituteAnalyzer : AbstractSubstituteAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax, ArgumentSyntax>
    {
        public SubstituteAnalyzer()
            : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance, SubstituteProxyAnalysis.Instance, SubstituteConstructorAnalysis.Instance, SubstituteConstructorMatcher.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override InvocationExpressionSyntax GetCorrespondingSubstituteInvocationExpressionSyntax(InvocationExpressionSyntax invocationExpressionSyntax, string substituteName)
        {
            var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)invocationExpressionSyntax.Expression;

            return invocationExpressionSyntax.WithExpression(
                memberAccessExpressionSyntax.WithName(
                    memberAccessExpressionSyntax.Name.WithIdentifier(Identifier(substituteName))));
        }

        protected override InvocationExpressionSyntax GetSubstituteInvocationExpressionSyntaxWithoutConstructorArguments(
            InvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol)
        {
            ArgumentListSyntax argumentListSyntax;
            if (methodSymbol.IsGenericMethod)
            {
                argumentListSyntax = ArgumentList();
            }
            else
            {
                var nullSyntax = Argument(LiteralExpression(SyntaxKind.NullLiteralExpression));
                argumentListSyntax = ArgumentList(SeparatedList(invocationExpressionSyntax.ArgumentList.Arguments.Take(1)).Add(nullSyntax));
            }

            return invocationExpressionSyntax.WithArgumentList(argumentListSyntax);
        }
    }
}