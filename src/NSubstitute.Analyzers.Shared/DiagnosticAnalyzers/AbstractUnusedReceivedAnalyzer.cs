using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractUnusedReceivedAnalyzer : AbstractDiagnosticAnalyzer
{
    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    protected AbstractUnusedReceivedAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        : base(diagnosticDescriptorsProvider)
    {
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.UnusedReceived);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    private static readonly ImmutableHashSet<OperationKind> PossibleParents =
        ImmutableHashSet.Create(OperationKind.PropertyReference, OperationKind.Invocation);

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

        if (invocationOperation.TargetMethod.IsReceivedLikeMethod() == false)
        {
            return;
        }

        if (IsConsideredAsUsed(invocationOperation))
        {
           return;
        }

        // even though we have TargetMethod in IInvocationOperation, it wont tell us if method was called as extension
        // or ordinary manner (and we need that information to correctly format diagnostic text)
        if (operationAnalysisContext.Compilation
                .GetSemanticModel(invocationOperation.Syntax.SyntaxTree)
                .GetSymbolInfo(invocationOperation.Syntax).Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        var diagnosticDescriptor = methodSymbol.MethodKind == MethodKind.Ordinary
            ? DiagnosticDescriptorsProvider.UnusedReceivedForOrdinaryMethod
            : DiagnosticDescriptorsProvider.UnusedReceived;

        var diagnostic = Diagnostic.Create(
            diagnosticDescriptor,
            invocationOperation.Syntax.GetLocation(),
            methodSymbol.Name,
            methodSymbol.ContainingType.Name);

        operationAnalysisContext.ReportDiagnostic(diagnostic);
    }

    private bool IsConsideredAsUsed(IOperation operation)
    {
        return operation.Parent != null && PossibleParents.Contains(operation.Parent.Kind);
    }
}