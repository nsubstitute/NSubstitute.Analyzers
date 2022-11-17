using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractReceivedInReceivedInOrderAnalyzer : AbstractDiagnosticAnalyzer
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;
    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected AbstractReceivedInReceivedInOrderAnalyzer(
        ISubstitutionNodeFinder substitutionNodeFinder,
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        : base(diagnosticDescriptorsProvider)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(diagnosticDescriptorsProvider.ReceivedUsedInReceivedInOrder);
    }

    protected sealed override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext operationAnalysisContext)
    {
        var invocationOperation = (IInvocationOperation)operationAnalysisContext.Operation;

        if (invocationOperation.TargetMethod.IsReceivedInOrderMethod() == false)
        {
            return;
        }

        foreach (var operation in _substitutionNodeFinder.FindForReceivedInOrderExpression(
                     operationAnalysisContext.Compilation,
                     invocationOperation,
                     includeAll: true).OfType<IInvocationOperation>())
        {
            if (operation.TargetMethod.IsReceivedLikeMethod() == false)
            {
               continue;
            }

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.ReceivedUsedInReceivedInOrder,
                operation.Syntax.GetLocation(),
                operation.TargetMethod.Name);

            operationAnalysisContext.ReportDiagnostic(diagnostic);
        }
    }
}