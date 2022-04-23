using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractReceivedInReceivedInOrderAnalyzer<TSyntaxKind> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;
    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected AbstractReceivedInReceivedInOrderAnalyzer(
        ISubstitutionNodeFinder substitutionNodeFinder,
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        : base(diagnosticDescriptorsProvider)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(diagnosticDescriptorsProvider.ReceivedUsedInReceivedInOrder);
    }

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        if (!(syntaxNodeContext.SemanticModel.GetOperation(syntaxNodeContext.Node) is IInvocationOperation invocationOperation))
        {
            return;
        }

        if (invocationOperation.TargetMethod.IsReceivedInOrderMethod() == false)
        {
            return;
        }

        foreach (var syntaxNode in _substitutionNodeFinder.FindForReceivedInOrderExpression(
                     syntaxNodeContext,
                     invocationOperation))
        {
            var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode);

            if (symbolInfo.Symbol.IsReceivedLikeMethod() == false)
            {
                continue;
            }

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.ReceivedUsedInReceivedInOrder,
                syntaxNode.GetLocation(),
                symbolInfo.Symbol.Name);

            syntaxNodeContext.ReportDiagnostic(diagnostic);
        }
    }
}