using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ISubstituteConstructorAnalysis<TInvocationExpression> where TInvocationExpression : SyntaxNode
{
    ConstructorContext CollectConstructorContext(SubstituteContext<TInvocationExpression> substituteContext, ITypeSymbol proxyTypeSymbol);
}