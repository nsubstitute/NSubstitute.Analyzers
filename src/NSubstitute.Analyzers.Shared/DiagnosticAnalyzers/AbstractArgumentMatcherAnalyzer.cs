using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractArgumentMatcherAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TMemberAccessExpression> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
        where TInvocationExpressionSyntax : SyntaxNode
        where TMemberAccessExpression : SyntaxNode
    {
        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractArgumentMatcherAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationContext =>
            {
                var compilationAnalyzer = CreateArgumentMatcherCompilationAnalyzer();

                compilationContext.RegisterSyntaxNodeAction(compilationAnalyzer.BeginAnalyzeArgMatchers, InvocationExpressionKind);

                compilationContext.RegisterCompilationEndAction(compilationAnalyzer.FinishAnalyzeArgMatchers);
            });
        }

        protected abstract AbstractArgumentMatcherCompilationAnalyzer<TInvocationExpressionSyntax, TMemberAccessExpression> CreateArgumentMatcherCompilationAnalyzer();
    }
}