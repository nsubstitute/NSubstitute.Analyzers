using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ISubstituteConstructorAnalysis
{
    ConstructorContext CollectConstructorContext(SubstituteContext substituteContext, ITypeSymbol proxyTypeSymbol);
}