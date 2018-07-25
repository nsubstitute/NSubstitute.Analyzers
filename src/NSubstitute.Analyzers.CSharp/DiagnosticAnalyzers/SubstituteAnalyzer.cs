using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SubstituteAnalyzer : AbstractSubstituteAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax, ArgumentSyntax>
    {
        public SubstituteAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

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
    }
}