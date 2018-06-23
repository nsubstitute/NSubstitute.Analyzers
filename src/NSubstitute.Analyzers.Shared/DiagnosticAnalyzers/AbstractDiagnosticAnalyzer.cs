using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;
using NSubstitute.Analyzers.Shared.Settings;

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
            if (IsSupressed(syntaxNodeContext, diagnostic, symbol))
            {
                return;
            }

            syntaxNodeContext.ReportDiagnostic(diagnostic);
        }

        protected bool IsSupressed(SyntaxNodeAnalysisContext syntaxNodeContext, Diagnostic diagnostic, ISymbol symbol)
        {
            if (symbol == null)
            {
                return false;
            }

            return IsSupressed(syntaxNodeContext, symbol, diagnostic.Id);
        }

        private bool IsSupressed(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol, string diagnosticId)
        {
            var analyzersSettings = syntaxNodeContext.GetSettings(CancellationToken.None);

            foreach (var supression in analyzersSettings.Suppressions.Where(suppression => suppression.Rules.Contains(diagnosticId)))
            {
                foreach (var supressedSymbol in DocumentationCommentId.GetSymbolsForDeclarationId(supression.Target, syntaxNodeContext.Compilation))
                {
                    if (supressedSymbol.Equals(symbol) ||
                        supressedSymbol.Equals(symbol.ContainingType) ||
                        supressedSymbol.Equals(symbol.ContainingNamespace) ||
                        (symbol is IMethodSymbol methodSymbol && methodSymbol.ConstructedFrom.Equals(supressedSymbol)) ||
                        (symbol.ContainingType is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.ConstructedFrom.Equals(supressedSymbol)) ||
                        (symbol is IPropertySymbol propertySymbol && propertySymbol.OriginalDefinition.Equals(supressedSymbol)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}