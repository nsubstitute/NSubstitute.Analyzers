using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractSyncOverAsyncThrowsAnalyzer : AbstractDiagnosticAnalyzer
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;
    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    protected AbstractSyncOverAsyncThrowsAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ISubstitutionNodeFinder substitutionNodeFinder)
        : base(diagnosticDescriptorsProvider)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
        SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.SyncOverAsyncThrows);
        _analyzeInvocationAction = AnalyzeInvocation;
    }

    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected sealed override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext operationAnalysisContext)
    {
        var invocationOperation = (IInvocationOperation)operationAnalysisContext.Operation;

        if (invocationOperation.TargetMethod.IsThrowSyncLikeMethod() == false)
        {
            return;
        }

        var substituteOperation = _substitutionNodeFinder.FindForStandardExpression(invocationOperation);

        if (substituteOperation == null)
        {
            return;
        }

        var returnType = GetReturnTypeSymbol(substituteOperation);

        if (IsTask(returnType, operationAnalysisContext.Compilation) == false)
        {
            return;
        }

        operationAnalysisContext.ReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptorsProvider.SyncOverAsyncThrows,
                invocationOperation.Syntax.GetLocation()));
    }

    private static ITypeSymbol GetReturnTypeSymbol(IOperation substituteOperation)
    {
        var returnType = substituteOperation switch
        {
            IInvocationOperation substituteInvocationOperation => substituteInvocationOperation.TargetMethod.ReturnType,
            IPropertyReferenceOperation propertyReferenceOperation => propertyReferenceOperation.Property.Type,
            IConversionOperation conversionOperation => GetReturnTypeSymbol(conversionOperation.Operand),
            _ => null
        };
        return returnType;
    }

    private static bool IsTask(ITypeSymbol returnType, Compilation compilation)
    {
        if (returnType == null)
        {
            return false;
        }

        var taskSymbol = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");

        if (taskSymbol == null)
        {
            return false;
        }

        return returnType.Equals(taskSymbol) ||
               (returnType.BaseType != null && returnType.BaseType.Equals(taskSymbol));
    }
}