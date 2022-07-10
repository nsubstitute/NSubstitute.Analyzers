using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberReceivedInOrderAnalyzer : AbstractNonSubstitutableSetupAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected ImmutableArray<OperationKind> IgnoredAncestorPaths { get; } = ImmutableArray.Create(
        OperationKind.VariableDeclarator,
        OperationKind.VariableDeclaration,
        OperationKind.EventAssignment,
        OperationKind.Argument);

    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;

    protected AbstractNonSubstitutableMemberReceivedInOrderAnalyzer(
        ISubstitutionNodeFinder substitutionNodeFinder,
        INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis,
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        : base(diagnosticDescriptorsProvider, nonSubstitutableMemberAnalysis)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        NonVirtualSetupDescriptor = diagnosticDescriptorsProvider.NonVirtualReceivedInOrderSetupSpecification;
        SupportedDiagnostics = ImmutableArray.Create(
            DiagnosticDescriptorsProvider.InternalSetupSpecification,
            DiagnosticDescriptorsProvider.NonVirtualReceivedInOrderSetupSpecification);
    }

    protected override DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

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

        foreach (var syntaxNode in _substitutionNodeFinder
                     .FindForReceivedInOrderExpression(operationAnalysisContext, invocationOperation)
                     .Where(operation => ShouldAnalyzeNode(operationAnalysisContext, operation)))
        {
            Analyze(operationAnalysisContext, syntaxNode);
        }
    }

    private bool ShouldAnalyzeNode(OperationAnalysisContext operationAnalysisContext, IOperation operation)
    {
        var maybeIgnoredOperation = FindIgnoredEnclosingOperation(operation);
        if (maybeIgnoredOperation == null)
        {
            return true;
        }

        if (maybeIgnoredOperation is IArgumentOperation &&
            maybeIgnoredOperation.Parent is IInvocationOperation invocationOperation &&
            invocationOperation.TargetMethod.IsReceivedInOrderMethod())
        {
            return true;
        }

        if (maybeIgnoredOperation.IsEventAssignmentOperation())
        {
            return false;
        }

        var symbol = GetVariableDeclaratorSymbol(maybeIgnoredOperation);

        if (symbol == null)
        {
            return false;
        }

        var blockOperation = maybeIgnoredOperation.Ancestors().OfType<IBlockOperation>().FirstOrDefault();

        if (blockOperation == null)
        {
            return false;
        }

        var semanticModel = operationAnalysisContext.Compilation.GetSemanticModel(blockOperation.Syntax.SyntaxTree);
        var dataFlowAnalysis = semanticModel.AnalyzeDataFlow(blockOperation.Syntax);
        return !dataFlowAnalysis.ReadInside.Contains(symbol);
    }

    private static ILocalSymbol GetVariableDeclaratorSymbol(IOperation operation)
    {
        return operation switch
        {
            IVariableDeclaratorOperation declarator => declarator.Symbol,
            IVariableDeclarationOperation declarationOperation => declarationOperation.Declarators.FirstOrDefault()?.Symbol,
            _ => null
        };
    }

    private IOperation FindIgnoredEnclosingOperation(IOperation operation) => operation.Ancestors()
        .FirstOrDefault(ancestor => IgnoredAncestorPaths.Contains(ancestor.Kind));
}