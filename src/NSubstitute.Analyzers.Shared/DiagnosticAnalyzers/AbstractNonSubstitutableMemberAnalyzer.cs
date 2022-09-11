using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberAnalyzer : AbstractNonSubstitutableSetupAnalyzer
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;

    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected AbstractNonSubstitutableMemberAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ISubstitutionNodeFinder substitutionNodeFinder,
        INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis)
        : base(diagnosticDescriptorsProvider, nonSubstitutableMemberAnalysis)
    {
        _analyzeInvocationAction = AnalyzeInvocation;
        _substitutionNodeFinder = substitutionNodeFinder;
        NonVirtualSetupDescriptor = diagnosticDescriptorsProvider.NonVirtualSetupSpecification;
        SupportedDiagnostics = ImmutableArray.Create(
            DiagnosticDescriptorsProvider.NonVirtualSetupSpecification,
            DiagnosticDescriptorsProvider.InternalSetupSpecification);
    }

    protected override DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

    protected sealed override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext operationAnalysisContext)
    {
        var invocationOperation = (IInvocationOperation)operationAnalysisContext.Operation;

        if (invocationOperation.TargetMethod.IsReturnOrThrowLikeMethod() == false)
        {
            return;
        }

        AnalyzeMember(operationAnalysisContext, _substitutionNodeFinder.FindForStandardExpression(invocationOperation));
    }

    private void AnalyzeMember(OperationAnalysisContext operationAnalysisContext, IOperation accessedMember)
    {
        if (IsValidForAnalysis(accessedMember) == false)
        {
            return;
        }

        Analyze(operationAnalysisContext, accessedMember);
    }

    private bool IsValidForAnalysis(IOperation accessedMember)
    {
        return accessedMember != null && accessedMember is not ILocalReferenceOperation &&
               accessedMember is not IConversionOperation
               {
                   Operand: ILocalReferenceOperation
               };
    }
}