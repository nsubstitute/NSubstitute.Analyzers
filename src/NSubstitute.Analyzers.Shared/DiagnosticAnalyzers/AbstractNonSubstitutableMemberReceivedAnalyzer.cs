using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonSubstitutableMemberReceivedAnalyzer<TSyntaxKind, TMemberAccessExpressionSyntax> : AbstractNonSubstitutableSetupAnalyzer
        where TSyntaxKind : struct
        where TMemberAccessExpressionSyntax : SyntaxNode
    {
        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected AbstractNonSubstitutableMemberReceivedAnalyzer(
            IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
            INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis)
            : base(diagnosticDescriptorsProvider, nonSubstitutableMemberAnalysis)
        {
            _analyzeInvocationAction = AnalyzeInvocation;
            SupportedDiagnostics = ImmutableArray.Create(
                DiagnosticDescriptorsProvider.NonVirtualReceivedSetupSpecification,
                DiagnosticDescriptorsProvider.InternalSetupSpecification);
            NonVirtualSetupDescriptor = diagnosticDescriptorsProvider.NonVirtualReceivedSetupSpecification;
        }

        protected abstract ImmutableHashSet<int> PossibleParentsRawKinds { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected override DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

        protected override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
        }

        protected override Location GetSubstitutionNodeActualLocation(in NonSubstitutableMemberAnalysisResult analysisResult)
        {
            return analysisResult.Member.GetSubstitutionNodeActualLocation<TMemberAccessExpressionSyntax>(analysisResult.Symbol);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = syntaxNodeContext.Node;
            if (!(syntaxNodeContext.SemanticModel.GetOperation(invocationExpression) is IInvocationOperation invocationOperation))
            {
                return;
            }

            if (invocationOperation.TargetMethod.IsReceivedLikeMethod() == false)
            {
                return;
            }

            var parentNode = GetKnownParent(invocationExpression);

            if (parentNode == null)
            {
                return;
            }

            var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(parentNode);

            if (symbolInfo.Symbol == null)
            {
                return;
            }

            Analyze(syntaxNodeContext, parentNode, symbolInfo.Symbol);
        }

        private SyntaxNode GetKnownParent(SyntaxNode receivedSyntaxNode)
        {
            return PossibleParentsRawKinds.Contains(receivedSyntaxNode.Parent.RawKind) ? receivedSyntaxNode.Parent : null;
        }
    }
}