using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractCallInfoFinder<TInvocationExpressionSyntax, TIndexerSyntax>
    {
        public abstract CallInfoContext<TInvocationExpressionSyntax, TIndexerSyntax> GetCallInfoContext(SemanticModel semanticModel, SyntaxNode syntaxNode);
    }
}