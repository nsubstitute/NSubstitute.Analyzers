using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberReceivedAnalyzer : AbstractNonSubstitutableSetupAnalyzer
{
    private readonly Action<OperationAnalysisContext> _analyzeInvocationOperation;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected AbstractNonSubstitutableMemberReceivedAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis)
        : base(diagnosticDescriptorsProvider, nonSubstitutableMemberAnalysis)
    {
        _analyzeInvocationOperation = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(
            DiagnosticDescriptorsProvider.NonVirtualReceivedSetupSpecification,
            DiagnosticDescriptorsProvider.InternalSetupSpecification);
        NonVirtualSetupDescriptor = diagnosticDescriptorsProvider.NonVirtualReceivedSetupSpecification;
    }

    protected override DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationOperation, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext syntaxNodeContext)
    {
        if (syntaxNodeContext.Operation is not IInvocationOperation invocationOperation)
        {
           return;
        }

        if (invocationOperation.Parent == null)
        {
            return;
        }

        if (invocationOperation.TargetMethod.IsReceivedLikeMethod() == false)
        {
            return;
        }

        Analyze(syntaxNodeContext, invocationOperation.Parent);
    }
}