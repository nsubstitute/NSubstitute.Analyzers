using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Settings;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class SyntaxNodeAnalysisContextExtensions
{
    internal static AnalyzersSettings GetSettings(this SyntaxNodeAnalysisContext context, CancellationToken cancellationToken)
    {
        return context.Options.GetSettings(cancellationToken);
    }

    internal static void TryReportDiagnostic(
        this SyntaxNodeAnalysisContext syntaxNodeContext,
        Diagnostic diagnostic,
        ISymbol symbol)
    {
        if (IsSuppressed(syntaxNodeContext, diagnostic, symbol))
        {
            return;
        }

        syntaxNodeContext.ReportDiagnostic(diagnostic);
    }

    private static bool IsSuppressed(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        Diagnostic diagnostic,
        ISymbol symbol)
    {
        return symbol != null && IsSuppressed(syntaxNodeContext, symbol, diagnostic.Id);
    }

    private static bool IsSuppressed(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        ISymbol symbol,
        string diagnosticId)
    {
        var analyzersSettings = syntaxNodeContext.GetSettings(CancellationToken.None);

        if (analyzersSettings.Suppressions.Count == 0)
        {
            return false;
        }

        var possibleSymbols = GetPossibleSymbols(symbol).ToImmutableHashSet();

        return analyzersSettings.Suppressions.Where(suppression => suppression.Rules.Contains(diagnosticId))
            .SelectMany(suppression =>
                DocumentationCommentId.GetSymbolsForDeclarationId(suppression.Target, syntaxNodeContext.Compilation))
            .Any(possibleSymbols.Contains);
    }

    private static IEnumerable<ISymbol> GetPossibleSymbols(ISymbol symbol)
    {
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

        if (symbol.ContainingType is INamedTypeSymbol namedTypeSymbol)
        {
            yield return namedTypeSymbol.ConstructedFrom;
        }

        if (symbol is IPropertySymbol propertySymbol)
        {
            yield return propertySymbol.OriginalDefinition;
        }
    }
}