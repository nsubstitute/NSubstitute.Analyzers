using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class
        AbstractAsyncThrowsAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;
        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractAsyncThrowsAnalyzer(
            IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
            ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder)
            : base(diagnosticDescriptorsProvider)
        {
            _substitutionNodeFinder = substitutionNodeFinder;
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.AsyncThrows);

            _analyzeInvocationAction = AnalyzeInvocation;
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

            if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;

            if (!methodSymbol.IsThrowLikeMethod())
            {
                return;
            }

            var substitutedExpression = _substitutionNodeFinder.FindForStandardExpression(
                (TInvocationExpressionSyntax)invocationExpression,
                methodSymbol);

            if (substitutedExpression == null)
            {
                return;
            }

            var semanticModel = syntaxNodeContext.SemanticModel.GetSymbolInfo(substitutedExpression);

            if (!(semanticModel.Symbol is IMethodSymbol substituteMethodSymbol))
            {
                return;
            }

            var typeByMetadataName = syntaxNodeContext.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");

            if (!HasTaskReturnType(substituteMethodSymbol, typeByMetadataName))
            {
                return;
            }

            syntaxNodeContext.ReportDiagnostic(
                Diagnostic.Create(DiagnosticDescriptorsProvider.AsyncThrows, invocationExpression.GetLocation()));
        }

        private static bool HasTaskReturnType(IMethodSymbol methodSymbol, ISymbol typeByMetadataName)
        {
            return methodSymbol.ReturnType.Equals(typeByMetadataName) ||
                   (methodSymbol.ReturnType.BaseType != null && methodSymbol.ReturnType.BaseType.Equals(typeByMetadataName));
        }
    }
}