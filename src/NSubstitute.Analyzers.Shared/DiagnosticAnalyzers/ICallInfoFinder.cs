using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ICallInfoFinder<TInvocationExpressionSyntax, TIndexerSyntax>
    where TInvocationExpressionSyntax : SyntaxNode where TIndexerSyntax : SyntaxNode
{
    CallInfoContext<TInvocationExpressionSyntax, TIndexerSyntax> GetCallInfoContext(SemanticModel semanticModel, SyntaxNode syntaxNode);
}