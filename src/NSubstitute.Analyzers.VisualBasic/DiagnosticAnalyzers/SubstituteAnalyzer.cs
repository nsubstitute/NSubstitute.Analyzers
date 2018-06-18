using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

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
    }
}