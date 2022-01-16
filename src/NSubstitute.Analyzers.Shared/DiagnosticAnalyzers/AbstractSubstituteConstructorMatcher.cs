using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractSubstituteConstructorMatcher : ISubstituteConstructorMatcher
{
    // even though conversion returns that key -> value is convertible it fails on the runtime when running through substitute creation
    private static IReadOnlyDictionary<SpecialType, SpecialType> WellKnownUnsupportedConversions { get; } =
        new Dictionary<SpecialType, SpecialType>
        {
            [SpecialType.System_Int16] = SpecialType.System_Decimal,
            [SpecialType.System_Int32] = SpecialType.System_Decimal,
            [SpecialType.System_Int64] = SpecialType.System_Decimal,
            [SpecialType.System_UInt16] = SpecialType.System_Decimal,
            [SpecialType.System_UInt32] = SpecialType.System_Decimal,
            [SpecialType.System_UInt64] = SpecialType.System_Decimal
        };

    private static IReadOnlyDictionary<SpecialType, HashSet<SpecialType>> WellKnownSupportedConversions { get; } =
        new Dictionary<SpecialType, HashSet<SpecialType>>
        {
            [SpecialType.System_Char] = new HashSet<SpecialType>
            {
                SpecialType.System_Int16,
                SpecialType.System_Int32,
                SpecialType.System_Int64,
                SpecialType.System_UInt16,
                SpecialType.System_UInt32,
                SpecialType.System_UInt64,
                SpecialType.System_Single
            }
        };

    public bool MatchesInvocation(Compilation compilation, IMethodSymbol methodSymbol, IList<ITypeSymbol> invocationParameters)
    {
        if (methodSymbol.Parameters.Length == 0)
        {
            return true;
        }

        return methodSymbol.Parameters.All(parameter =>
            MatchesInvocation(compilation, parameter, invocationParameters));
    }

    protected abstract bool IsConvertible(Compilation compilation, ITypeSymbol source, ITypeSymbol destination);

    // TODO simplify once https://github.com/nsubstitute/NSubstitute.Analyzers/issues/153 is implemented
    private bool MatchesInvocation(Compilation compilation, IParameterSymbol symbol, IList<ITypeSymbol> invocationParameters)
    {
        if (!symbol.IsParams)
        {
            return symbol.Ordinal < invocationParameters.Count &&
                   ClassifyConversion(compilation, invocationParameters[symbol.Ordinal], symbol.Type);
        }

        if (!(symbol.Type is IArrayTypeSymbol arrayTypeSymbol))
        {
            return false;
        }

        if (symbol.Ordinal >= invocationParameters.Count)
        {
            return true;
        }

        return invocationParameters
            .Where((typeSymbol, index) => index >= symbol.Ordinal).All(invocationSymbol =>
                ClassifyConversion(compilation, invocationSymbol, arrayTypeSymbol.ElementType));
    }

    private bool ClassifyConversion(Compilation compilation, ITypeSymbol source, ITypeSymbol destination)
    {
        if (source == null || source.Equals(destination))
        {
            return true;
        }

        if (WellKnownUnsupportedConversions.TryGetValue(source.SpecialType, out var unsupportedSource) && unsupportedSource == destination.SpecialType)
        {
            return false;
        }

        if (WellKnownSupportedConversions.TryGetValue(source.SpecialType, out var supportedSource) && supportedSource.Contains(destination.SpecialType))
        {
            return true;
        }

        return IsConvertible(compilation, source, destination);
    }
}