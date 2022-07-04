using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberWhenAnalyzer : AbstractNonSubstitutableSetupAnalyzer
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

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
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext operationAnalysisContext)
    {
        if (operationAnalysisContext.Operation is not IInvocationOperation invocationOperation)
        {
            return;
        }

        if (invocationOperation.TargetMethod.IsWhenLikeMethod() == false)
        {
            return;
        }

        var operations = _substitutionNodeFinder.FindForWhenExpression(operationAnalysisContext, invocationOperation);
        foreach (var operation in operations)
        {
            Analyze(operationAnalysisContext, operation);
        }
    }
}