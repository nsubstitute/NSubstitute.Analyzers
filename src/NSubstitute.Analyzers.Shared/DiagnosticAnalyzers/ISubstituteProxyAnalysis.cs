using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ISubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax> where TInvocationExpressionSyntax : SyntaxNode where TExpressionSyntax : SyntaxNode
{
    ITypeSymbol GetActualProxyTypeSymbol(SubstituteContext<TInvocationExpressionSyntax> substituteContext);

    ImmutableArray<ITypeSymbol> GetProxySymbols(SubstituteContext<TInvocationExpressionSyntax> substituteContext);

    ITypeSymbol GetActualProxyTypeSymbol(SemanticModel semanticModel, TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol);

    ImmutableArray<ITypeSymbol> GetProxySymbols(SemanticModel semanticModel, TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol);
}