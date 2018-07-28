using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal class SubstituteAnalyzer : AbstractSubstituteAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax, ArgumentSyntax>
    {
        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        public SubstituteAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override AbstractSubstituteProxyAnalysis<InvocationExpressionSyntax, ExpressionSyntax> GetSubstituteProxyAnalysis()
        {
            return new SubstituteProxyAnalysis();
        }

        protected override AbstractSubstituteConstructorAnalysis<InvocationExpressionSyntax, ArgumentSyntax> GetSubstituteConstructorAnalysis()
        {
            return new SubstituteConstructorAnalysis();
        }

        protected override AbstractSubstituteConstructorMatcher GetSubstituteConstructorMatcher()
        {
            return new SubstituteConstructorMatcher();
        }

        protected override InvocationExpressionSyntax GetCorrespondingSubstituteInvocationExpressionSyntax(InvocationExpressionSyntax invocationExpressionSyntax, string substituteName)
        {
            var simpleNameSyntax = (SimpleNameSyntax)invocationExpressionSyntax.Expression;
            return invocationExpressionSyntax.WithExpression(simpleNameSyntax.WithIdentifier(IdentifierName(substituteName).Identifier));
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
}