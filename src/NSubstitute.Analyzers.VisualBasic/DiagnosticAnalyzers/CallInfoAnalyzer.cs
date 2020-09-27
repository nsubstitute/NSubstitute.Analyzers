using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal sealed class CallInfoAnalyzer : AbstractCallInfoAnalyzer<SyntaxKind, InvocationExpressionSyntax, InvocationExpressionSyntax>
    {
        public CallInfoAnalyzer()
            : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, CallInfoCallFinder.Instance, SubstitutionNodeFinder.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax indexerExpressionSyntax)
        {
            return syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(indexerExpressionSyntax).Symbol ??
                   syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(indexerExpressionSyntax.Expression).Symbol;
        }

        protected override bool CanCast(Compilation compilation, ITypeSymbol sourceSymbol, ITypeSymbol destinationSymbol)
        {
            return compilation.ClassifyConversion(sourceSymbol, destinationSymbol).Exists;
        }

        protected override bool IsAssignableTo(Compilation compilation, ITypeSymbol fromSymbol, ITypeSymbol toSymbol)
        {
            var conversion = compilation.ClassifyConversion(fromSymbol, toSymbol);
            return conversion.Exists && conversion.IsNarrowing == false && conversion.IsNumeric == false;
        }
    }
}