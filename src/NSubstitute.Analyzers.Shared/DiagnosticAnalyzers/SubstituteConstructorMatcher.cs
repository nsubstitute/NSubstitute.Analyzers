using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    // TODO refactor
    public static class SubstituteConstructorMatcher
    {
        // even though conversion returns that key -> value is convertible it fails on the runtime when runninig through substitute creation
        private static readonly Dictionary<SpecialType, SpecialType> WellKnownUnsupportedConversions =
            new Dictionary<SpecialType, SpecialType>
            {
                [SpecialType.System_Int16] = SpecialType.System_Decimal,
                [SpecialType.System_Int32] = SpecialType.System_Decimal,
                [SpecialType.System_Int64] = SpecialType.System_Decimal,
                [SpecialType.System_UInt16] = SpecialType.System_Decimal,
                [SpecialType.System_UInt32] = SpecialType.System_Decimal,
                [SpecialType.System_UInt64] = SpecialType.System_Decimal
            };

        private static readonly Dictionary<SpecialType, HashSet<SpecialType>> WellKnownSupportedConversions =
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

        public static bool MatchesInvocation(Compilation compilation, IMethodSymbol methodSymbol, IList<ITypeSymbol> invocationParameters)
        {
            if (methodSymbol.Parameters.Length != invocationParameters.Count)
            {
                return false;
            }

            return methodSymbol.Parameters.Length == 0 || methodSymbol.Parameters
                       .Where((symbol, index) => IsConvertible(compilation, invocationParameters[index], symbol.Type))
                       .Count() == methodSymbol.Parameters.Length;
        }

        private static bool IsConvertible(Compilation compilation, ITypeSymbol source, ITypeSymbol destination)
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

            var conversion = compilation.ClassifyConversion(source, destination);

#if CSHARP
                return conversion.Exists && conversion.IsImplicit;

#elif VISUAL_BASIC
            // TODO lack of conversion.IsImplicit in VB Conversion object
            return conversion.Exists && conversion.IsNarrowing == false;
#endif
        }
    }
}