using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberWhenAnalyzer<TSyntaxKind> : AbstractNonSubstitutableSetupAnalyzer
    where TSyntaxKind : struct, Enum
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

    protected AbstractNonSubstitutableMemberWhenAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ISubstitutionNodeFinder substitutionNodeFinder,
        INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis)
        : base(diagnosticDescriptorsProvider, nonSubstitutableMemberAnalysis)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(
            DiagnosticDescriptorsProvider.NonVirtualWhenSetupSpecification,
            DiagnosticDescriptorsProvider.InternalSetupSpecification);
        NonVirtualSetupDescriptor = diagnosticDescriptorsProvider.NonVirtualWhenSetupSpecification;
    }

    protected override DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        if (!(syntaxNodeContext.SemanticModel.GetOperation(syntaxNodeContext.Node) is IInvocationOperation
                invocationOperation))
        {
            return;
        }

        if (invocationOperation.TargetMethod.IsWhenLikeMethod() == false)
        {
            return;
        }

        var expressionsForAnalysys =
            _substitutionNodeFinder.FindForWhenExpression(syntaxNodeContext, invocationOperation);
        foreach (var analysedSyntax in expressionsForAnalysys)
        {
            var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(analysedSyntax);
            if (symbolInfo.Symbol != null)
            {
                Analyze(syntaxNodeContext, analysedSyntax, symbolInfo.Symbol);
            }
        }
    }
}