using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractArgumentMatcherAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TMemberAccessExpressionSyntax, TArgumentSyntax> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
        where TInvocationExpressionSyntax : SyntaxNode
        where TMemberAccessExpressionSyntax : SyntaxNode
        where TArgumentSyntax : SyntaxNode
    {
        private readonly Action<CompilationStartAnalysisContext> _compilationStartAction;

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractArgumentMatcherAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.ArgumentMatcherUsedWithoutSpecifyingCall);
            _compilationStartAction = AnalyzeCompilation;
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(_compilationStartAction);
        }

        protected abstract AbstractArgumentMatcherCompilationAnalyzer<TInvocationExpressionSyntax, TMemberAccessExpressionSyntax, TArgumentSyntax> CreateArgumentMatcherCompilationAnalyzer();

        private void AnalyzeCompilation(CompilationStartAnalysisContext compilationContext)
        {
            var compilationAnalyzer = CreateArgumentMatcherCompilationAnalyzer();

            compilationContext.RegisterSyntaxNodeAction(compilationAnalyzer.BeginAnalyzeArgMatchers, InvocationExpressionKind);

            compilationContext.RegisterCompilationEndAction(compilationAnalyzer.FinishAnalyzeArgMatchers);
        }
    }
}