using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ISubstituteProxyAnalysis
{
    ITypeSymbol? GetActualProxyTypeSymbol(IInvocationOperation invocationOperation);

    ImmutableArray<ITypeSymbol> GetProxySymbols(IInvocationOperation invocationOperation);
}