using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractReEntrantSetupAnalyzer : AbstractDiagnosticAnalyzer
{
    private readonly IReEntrantCallFinder _reEntrantCallFinder;
    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected AbstractReEntrantSetupAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        IReEntrantCallFinder reEntrantCallFinder)
        : base(diagnosticDescriptorsProvider)
    {
        _reEntrantCallFinder = reEntrantCallFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.ReEntrantSubstituteCall);
    }

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext context)
    {
        if (context.Operation is not IInvocationOperation invocationOperation)
        {
           return;
        }

        if (invocationOperation.TargetMethod.IsInitialReEntryLikeMethod() == false)
        {
           return;
        }

        var argumentOperations = invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument();

        foreach (var argumentOperation in argumentOperations)
        {
            if (IsPassedByParamsArrayOfCallInfoFunc(context.Compilation, argumentOperation))
            {
                continue;
            }

            if (IsPassedByParamsArray(argumentOperation))
            {
                AnalyzeParamsArgument(context, argumentOperation, invocationOperation);
            }
            else
            {
                AnalyzeExpression(context, argumentOperation.Value, invocationOperation);
            }
        }
    }

    private void AnalyzeParamsArgument(
        OperationAnalysisContext context,
        IArgumentOperation argumentOperation,
        IInvocationOperation invocationOperation)
    {
        var initializerOperations = argumentOperation.Value.GetArrayElementValues();

        if (initializerOperations != null)
        {
            foreach (var operation in initializerOperations)
            {
                AnalyzeExpression(context, operation, invocationOperation);
            }

            return;
        }

        // if array elements can't be extracted, analyze argument itself
        AnalyzeExpression(context, argumentOperation.Value, invocationOperation);
    }

    private void AnalyzeExpression(
        OperationAnalysisContext context,
        IOperation operation,
        IInvocationOperation invocationOperation)
    {
        var reentrantSymbol = _reEntrantCallFinder.GetReEntrantCalls(
            context.Compilation,
            invocationOperation,
            operation).FirstOrDefault();

        if (reentrantSymbol != null)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.ReEntrantSubstituteCall,
                operation.Syntax.GetLocation(),
                invocationOperation.TargetMethod.Name,
                reentrantSymbol.ExtractSymbol()?.Name ?? string.Empty,
                operation.Syntax.ToString());

            context.ReportDiagnostic(diagnostic);
        }
    }

    private bool IsPassedByParamsArray(IArgumentOperation argumentOperation)
    {
        return argumentOperation != null &&
               argumentOperation.Parameter.IsParams &&
               argumentOperation.Parameter.Type is IArrayTypeSymbol;
    }

    private bool IsPassedByParamsArrayOfCallInfoFunc(Compilation compilation, IArgumentOperation argumentOperation)
    {
        return IsPassedByParamsArray(argumentOperation) &&
               argumentOperation.Parameter.Type is IArrayTypeSymbol arrayTypeSymbol &&
               arrayTypeSymbol.ElementType is INamedTypeSymbol namedTypeSymbol &&
               namedTypeSymbol.IsCallInfoDelegate(compilation);
    }
}