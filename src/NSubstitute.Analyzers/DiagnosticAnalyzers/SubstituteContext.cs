using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
#if CSHARP
using Microsoft.CodeAnalysis.CSharp.Syntax;
#elif VISUAL_BASIC
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

namespace NSubstitute.Analyzers.DiagnosticAnalyzers
{
    public readonly struct SubstituteContext
    {
        public SyntaxNodeAnalysisContext SyntaxNodeAnalysisContext { get; }

        public InvocationExpressionSyntax InvocationExpression { get; }

        public IMethodSymbol MethodSymbol { get; }

        public SubstituteContext(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            SyntaxNodeAnalysisContext = syntaxNodeAnalysisContext;
            InvocationExpression = invocationExpression;
            MethodSymbol = methodSymbol;
        }
    }
}