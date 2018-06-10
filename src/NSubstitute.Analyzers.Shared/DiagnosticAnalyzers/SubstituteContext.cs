using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    public struct SubstituteContext<TInvocationExpression>
        where TInvocationExpression : SyntaxNode
    {
        public SyntaxNodeAnalysisContext SyntaxNodeAnalysisContext { get; }

        public TInvocationExpression InvocationExpression { get; }

        public IMethodSymbol MethodSymbol { get; }

        public SubstituteContext(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TInvocationExpression invocationExpression, IMethodSymbol methodSymbol)
        {
            SyntaxNodeAnalysisContext = syntaxNodeAnalysisContext;
            InvocationExpression = invocationExpression;
            MethodSymbol = methodSymbol;
        }
    }
}