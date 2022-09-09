using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractAsyncReceivedInOrderCallbackAnalyzer : AbstractDiagnosticAnalyzer
{
    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    protected AbstractAsyncReceivedInOrderCallbackAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        : base(diagnosticDescriptorsProvider)
    {
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(diagnosticDescriptorsProvider.AsyncCallbackUsedInReceivedInOrder);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected abstract SyntaxToken? GetAsyncToken(SyntaxNode node);

    protected sealed override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext operationAnalysisContext)
    {
        if (operationAnalysisContext.Operation is not IInvocationOperation invocationOperation)
        {
           return;
        }

        if (invocationOperation.TargetMethod.IsReceivedInOrderMethod() == false)
        {
            return;
        }

        foreach (var invocationOperationArgument in invocationOperation.Arguments)
        {
            var asyncToken = GetAsyncToken(invocationOperationArgument.Value.Syntax);

            if (asyncToken.HasValue == false)
            {
                continue;
            }

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.AsyncCallbackUsedInReceivedInOrder,
                asyncToken.Value.GetLocation());

            operationAnalysisContext.ReportDiagnostic(diagnostic);
        }
    }
}