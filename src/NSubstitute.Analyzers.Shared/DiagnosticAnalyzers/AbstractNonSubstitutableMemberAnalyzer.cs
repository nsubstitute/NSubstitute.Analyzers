using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonSubstitutableMemberAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractNonSubstitutableSetupAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;

        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected abstract ImmutableHashSet<int> SupportedMemberAccesses { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractNonSubstitutableMemberAnalyzer(
            IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
            ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder,
            INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis)
            : base(diagnosticDescriptorsProvider, nonSubstitutableMemberAnalysis)
        {
            _analyzeInvocationAction = AnalyzeInvocation;
            _substitutionNodeFinder = substitutionNodeFinder;
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.NonVirtualSetupSpecification, DiagnosticDescriptorsProvider.InternalSetupSpecification);
            NonVirtualSetupDescriptor = diagnosticDescriptorsProvider.NonVirtualSetupSpecification;
        }

        protected override DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

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

            if (methodSymbol.IsReturnOrThrowLikeMethod() == false)
            {
                return;
            }

            AnalyzeMember(syntaxNodeContext, _substitutionNodeFinder.FindForStandardExpression((TInvocationExpressionSyntax)invocationExpression, methodSymbol));
        }

        private void AnalyzeMember(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode accessedMember)
        {
            if (IsValidForAnalysis(accessedMember) == false)
            {
                return;
            }

            Analyze(syntaxNodeContext, accessedMember);
        }

        private bool IsValidForAnalysis(SyntaxNode accessedMember)
        {
            return accessedMember != null && SupportedMemberAccesses.Contains(accessedMember.RawKind);
        }
    }
}