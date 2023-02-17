using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Settings;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class OperationAnalysisContextExtensions
{
    internal static void TryReportDiagnostic(
        this OperationAnalysisContext syntaxNodeContext,
        Diagnostic diagnostic,
        ISymbol? symbol)
    {
        if (IsSuppressed(
                syntaxNodeContext.Options.GetSettings(CancellationToken.None),
                syntaxNodeContext.Compilation,
                symbol,
                diagnostic.Id))
        {
            return;
        }

        syntaxNodeContext.ReportDiagnostic(diagnostic);
    }

    private static bool IsSuppressed(
        AnalyzersSettings analyzersSettings,
        Compilation compilation,
        ISymbol? symbol,
        string diagnosticId)
    {
        if (analyzersSettings.Suppressions.Count == 0)
        {
            return false;
        }

        var possibleSymbols = GetPossibleSymbols(symbol).ToImmutableHashSet();

        return analyzersSettings.Suppressions.Where(suppression => suppression.Rules.Contains(diagnosticId))
            .SelectMany(suppression =>
                DocumentationCommentId.GetSymbolsForDeclarationId(suppression.Target, compilation))
            .Any(possibleSymbols.Contains);
    }

    private static IEnumerable<ISymbol> GetPossibleSymbols(ISymbol? symbol)
    {
        if (symbol == null)
        {
           yield break;
        }

        yield return symbol;
        yield return symbol.ContainingType;
        yield return symbol.ContainingNamespace;

        if (symbol is IMethodSymbol methodSymbol)
        {
            yield return methodSymbol.ConstructedFrom;
            if (methodSymbol.ReducedFrom != null)
            {
                yield return methodSymbol.ReducedFrom;
            }
        }

        if (symbol.ContainingType != null)
        {
            yield return symbol.ContainingType.ConstructedFrom;
        }

        if (symbol is IPropertySymbol propertySymbol)
        {
            yield return propertySymbol.OriginalDefinition;
        }
    }
}