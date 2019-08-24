using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal interface ISubstituteConstructorMatcher
    {
        bool MatchesInvocation(Compilation compilation, IMethodSymbol methodSymbol, IList<ITypeSymbol> invocationParameters);
    }
}