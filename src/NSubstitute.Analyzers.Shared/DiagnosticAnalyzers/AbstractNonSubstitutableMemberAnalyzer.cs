using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonSubstitutableMemberAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;

        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected abstract ImmutableHashSet<int> SupportedMemberAccesses { get; }

        protected abstract ImmutableHashSet<Type> KnownNonVirtualSyntaxKinds { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractNonSubstitutableMemberAnalyzer(
            IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
            ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder)
            : base(diagnosticDescriptorsProvider)
        {
            _analyzeInvocationAction = AnalyzeInvocation;
            _substitutionNodeFinder = substitutionNodeFinder;
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.NonVirtualSetupSpecification, DiagnosticDescriptorsProvider.InternalSetupSpecification);
        }

        protected override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
        }

        protected virtual bool? CanBeSetuped(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode accessedMember, SymbolInfo symbolInfo)
        {
            if (KnownNonVirtualSyntaxKinds.Contains(accessedMember.GetType()))
            {
                return false;
            }

            return symbolInfo.Symbol?.CanBeSetuped();
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

            if (methodSymbol.IsSetUpLikeMethod() == false)
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

            var accessedSymbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(accessedMember);

            var canBeSetuped = CanBeSetuped(syntaxNodeContext, accessedMember, accessedSymbol);
            if (canBeSetuped.HasValue && canBeSetuped == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.NonVirtualSetupSpecification,
                    accessedMember.GetLocation(),
                    accessedSymbol.Symbol?.Name ?? accessedMember.ToString());

                TryReportDiagnostic(syntaxNodeContext, diagnostic, accessedSymbol.Symbol);
            }

            if (accessedSymbol.Symbol != null && canBeSetuped.HasValue && canBeSetuped == true && accessedSymbol.Symbol.MemberVisibleToProxyGenerator() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.InternalSetupSpecification,
                    accessedMember.GetLocation(),
                    accessedSymbol.Symbol.Name);

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }

        private bool IsValidForAnalysis(SyntaxNode accessedMember)
        {
            return accessedMember != null && SupportedMemberAccesses.Contains(accessedMember.RawKind);
        }
    }
}