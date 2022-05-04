using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberAnalyzer<TSyntaxKind> : AbstractNonSubstitutableSetupAnalyzer
    where TSyntaxKind : struct
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;

    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

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
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        if (!(syntaxNodeContext.SemanticModel.GetOperation(syntaxNodeContext.Node) is IInvocationOperation
                invocationOperation))
        {
            return;
        }

        if (invocationOperation.TargetMethod.IsReturnOrThrowLikeMethod() == false)
        {
            return;
        }

        AnalyzeMember(syntaxNodeContext, _substitutionNodeFinder.FindOperationForStandardExpression(invocationOperation));
    }

    private void AnalyzeMember(SyntaxNodeAnalysisContext syntaxNodeContext, IOperation accessedMember)
    {
        if (IsValidForAnalysis(accessedMember) == false)
        {
            return;
        }

        Analyze(syntaxNodeContext, accessedMember.Syntax);
    }

    // TODO use switch expressions/pattern matching when GH-179 merged
    private bool IsValidForAnalysis(IOperation accessedMember)
    {
        if (accessedMember == null)
        {
            return false;
        }

        if (accessedMember is ILocalReferenceOperation)
        {
            return false;
        }

        if (accessedMember is IConversionOperation conversionOperation &&
            conversionOperation.Operand is ILocalReferenceOperation)
        {
            return false;
        }

        return true;
    }
}