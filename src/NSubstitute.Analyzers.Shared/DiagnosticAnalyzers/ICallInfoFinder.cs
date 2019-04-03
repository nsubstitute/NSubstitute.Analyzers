using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal interface ICallInfoFinder<TInvocationExpressionSyntax, TIndexerSyntax>
    {
        CallInfoContext<TInvocationExpressionSyntax, TIndexerSyntax> GetCallInfoContext(SemanticModel semanticModel, SyntaxNode syntaxNode);
    }
}