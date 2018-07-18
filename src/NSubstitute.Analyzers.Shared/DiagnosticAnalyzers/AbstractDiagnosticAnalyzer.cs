using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        protected IDiagnosticDescriptorsProvider DiagnosticDescriptorsProvider { get; }

        protected AbstractDiagnosticAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        {
            DiagnosticDescriptorsProvider = diagnosticDescriptorsProvider;
        }

        protected void TryReportDiagnostic(SyntaxNodeAnalysisContext syntaxNodeContext, Diagnostic diagnostic, ISymbol symbol)
        {
            if (IsSuppressed(syntaxNodeContext, diagnostic, symbol))
            {
                return;
            }

            syntaxNodeContext.ReportDiagnostic(diagnostic);
        }

        protected bool IsSuppressed(SyntaxNodeAnalysisContext syntaxNodeContext, Diagnostic diagnostic, ISymbol symbol)
        {
            return symbol != null && IsSuppressed(syntaxNodeContext, symbol, diagnostic.Id);
        }

        private bool IsSuppressed(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol, string diagnosticId)
        {
            var analyzersSettings = syntaxNodeContext.GetSettings(CancellationToken.None);
            var possibleSymbols = GetPossibleSymbols(symbol).ToList();

            return analyzersSettings.Suppressions.Where(suppression => suppression.Rules.Contains(diagnosticId))
                .SelectMany(suppression => DocumentationCommentId.GetSymbolsForDeclarationId(suppression.Target, syntaxNodeContext.Compilation))
                .Any(possibleSymbols.Contains);
        }

        private IEnumerable<ISymbol> GetPossibleSymbols(ISymbol symbol)
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
}