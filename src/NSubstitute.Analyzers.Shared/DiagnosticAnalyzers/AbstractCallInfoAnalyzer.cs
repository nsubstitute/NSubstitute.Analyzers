using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractCallInfoAnalyzer : AbstractDiagnosticAnalyzer
{
    private readonly ICallInfoFinder _callInfoFinder;
    private readonly ISubstitutionOperationFinder _substitutionOperationFinder;
    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    protected AbstractCallInfoAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ICallInfoFinder callInfoFinder,
        ISubstitutionOperationFinder substitutionOperationFinder)
        : base(diagnosticDescriptorsProvider)
    {
        _callInfoFinder = callInfoFinder;
        _substitutionOperationFinder = substitutionOperationFinder;
        _analyzeInvocationAction = AnalyzeInvocation;

        SupportedDiagnostics = ImmutableArray.Create(
            DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
            DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
            DiagnosticDescriptorsProvider.CallInfoCouldNotFindArgumentToThisCall,
            DiagnosticDescriptorsProvider.CallInfoMoreThanOneArgumentOfType,
            DiagnosticDescriptorsProvider.CallInfoArgumentSetWithIncompatibleValue,
            DiagnosticDescriptorsProvider.CallInfoArgumentIsNotOutOrRef);
    }

    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected sealed override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    protected abstract bool CanCast(Compilation compilation, ITypeSymbol sourceSymbol, ITypeSymbol destinationSymbol);

    protected abstract bool IsAssignableTo(Compilation compilation, ITypeSymbol fromSymbol, ITypeSymbol toSymbol);

    private bool SupportsCallInfo(Compilation compilation, IInvocationOperation invocationOperation)
    {
        if (invocationOperation.TargetMethod.IsCallInfoSupportingMethod() == false)
        {
            return false;
        }

        // perf - dont use linq in hotpath
        foreach (var arg in invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument())
        {
            if (arg.GetTypeSymbol().IsCallInfoDelegate(compilation))
            {
                return true;
            }
        }

        return false;
    }

    private void AnalyzeInvocation(OperationAnalysisContext operationAnalysisContext)
    {
        var invocationOperation = (IInvocationOperation)operationAnalysisContext.Operation;

        if (SupportsCallInfo(operationAnalysisContext.Compilation, invocationOperation) == false)
        {
            return;
        }

        var substitute = GetSubstitute(operationAnalysisContext, invocationOperation);

        if (substitute == null)
        {
            return;
        }

        foreach (var argumentExpressionSyntax in
                 invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument())
        {
            var callInfoContext = _callInfoFinder.GetCallInfoContext(argumentExpressionSyntax);

            AnalyzeArgAtInvocations(operationAnalysisContext, callInfoContext, substitute);

            AnalyzeArgInvocations(operationAnalysisContext, callInfoContext, substitute);

            AnalyzeIndexerInvocations(operationAnalysisContext, callInfoContext, substitute);
        }
    }

    private void AnalyzeIndexerInvocations(OperationAnalysisContext operationAnalysisContext, CallInfoContext callInfoContext, Substitute substitute)
    {
        foreach (var indexer in callInfoContext.IndexerAccessesOperations)
        {
            var indexerInfo = GetIndexerInfo(indexer);

            var position = indexer.GetIndexerPosition();

            if (AnalyzeArgumentAccess(operationAnalysisContext, substitute, indexer, position))
            {
                continue;
            }

            if (AnalyzeCast(operationAnalysisContext, substitute, indexer, in indexerInfo, position))
            {
                continue;
            }

            AnalyzeAssignment(operationAnalysisContext, substitute, indexer, indexerInfo, position);
        }
    }

    private void AnalyzeArgAtInvocations(OperationAnalysisContext operationAnalysisContext, CallInfoContext callInfoContext, Substitute substitute)
    {
        foreach (var argAtInvocation in callInfoContext.ArgAtInvocationsOperations)
        {
            var position = argAtInvocation.GetIndexerPosition();

            if (position.HasValue == false)
            {
                continue;
            }

            if (substitute.HasCallArgumentAt(position.Value) == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
                    argAtInvocation.Syntax.GetLocation(),
                    position);

                operationAnalysisContext.ReportDiagnostic(diagnostic);
                continue;
            }

            var typeSymbol = substitute.GetCallArgumentAt(position.Value).ArgumentTypeSymbol;
            if (typeSymbol.IsArgAnyType(operationAnalysisContext.Compilation) == false && IsAssignableTo(
                    operationAnalysisContext.Compilation,
                    typeSymbol,
                    argAtInvocation.TargetMethod.TypeArguments.First()) == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
                    argAtInvocation.Syntax.GetLocation(),
                    position,
                    argAtInvocation.TargetMethod.TypeArguments.First());

                operationAnalysisContext.ReportDiagnostic(diagnostic);
            }
        }
    }

    private void AnalyzeArgInvocations(OperationAnalysisContext operationAnalysisContext, CallInfoContext callInfoContext, Substitute substitute)
    {
        foreach (var argInvocationOperation in callInfoContext.ArgInvocationsOperations)
        {
            var typeSymbol = argInvocationOperation.TargetMethod.TypeArguments.First();
            var parameterCount =
                GetMatchingParametersCount(operationAnalysisContext.Compilation, substitute, typeSymbol);
            if (parameterCount == 0)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoCouldNotFindArgumentToThisCall,
                    argInvocationOperation.Syntax.GetLocation(),
                    typeSymbol);

                operationAnalysisContext.ReportDiagnostic(diagnostic);
                continue;
            }

            if (parameterCount > 1)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoMoreThanOneArgumentOfType,
                    argInvocationOperation.Syntax.GetLocation(),
                    typeSymbol);

                operationAnalysisContext.ReportDiagnostic(diagnostic);
            }
        }
    }

    private bool AnalyzeArgumentAccess(
        OperationAnalysisContext syntaxNodeContext,
        Substitute substitute,
        IOperation indexerOperation,
        int? position)
    {
        if (!position.HasValue || substitute.HasCallArgumentAt(position.Value))
        {
            return false;
        }

        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
            indexerOperation.Syntax.GetLocation(),
            position.Value);

        syntaxNodeContext.ReportDiagnostic(diagnostic);
        return true;
    }

    private bool AnalyzeCast(OperationAnalysisContext operationAnalysisContext, Substitute substitute, IOperation indexer, in IndexerInfo indexerInfo, int? position)
    {
        if (!position.HasValue || !indexerInfo.VerifyIndexerCast)
        {
            return false;
        }

        if (indexer.Parent is not IConversionOperation conversionOperation)
        {
            return false;
        }

        var type = conversionOperation.Type;
        var substituteParameterTypeSymbol = substitute.GetCallArgumentAt(position.Value).ArgumentTypeSymbol;
        if (type != null && substituteParameterTypeSymbol.IsArgAnyType(operationAnalysisContext.Compilation) == false &&
            CanCast(operationAnalysisContext.Compilation, substituteParameterTypeSymbol, type) == false)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
                indexer.Syntax.GetLocation(),
                position.Value,
                type);
            operationAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeAssignment(
        OperationAnalysisContext operationAnalysisContext,
        Substitute substitute,
        IOperation indexerOperation,
        in IndexerInfo indexerInfo,
        int? position)
    {
        if (!indexerInfo.VerifyAssignment || !position.HasValue || !substitute.HasCallArgumentAt(position.Value))
        {
            return false;
        }

        if (indexerOperation is IPropertyReferenceOperation { Parent: ISimpleAssignmentOperation simpleAssignmentOperation })
        {
            var callArgument = substitute.GetCallArgumentAt(position.Value);
            if (callArgument.ArgumentOperation == null ||
                (callArgument.ArgumentOperation.Parameter.RefKind != RefKind.Out &&
                 callArgument.ArgumentOperation.Parameter.RefKind != RefKind.Ref))
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoArgumentIsNotOutOrRef,
                    indexerOperation.Syntax.GetLocation(),
                    position.Value,
                    callArgument.ParameterTypeSymbol);
                operationAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }

            var assignmentType = simpleAssignmentOperation.GetTypeSymbol();
            var typeSymbol = substitute.GetCallArgumentAt(position.Value).ArgumentTypeSymbol;
            if (assignmentType != null &&
                IsAssignableTo(operationAnalysisContext.Compilation, assignmentType, typeSymbol) == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoArgumentSetWithIncompatibleValue,
                    indexerOperation.Syntax.GetLocation(),
                    assignmentType,
                    position.Value,
                    typeSymbol);
                operationAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }
        }

        return false;
    }

    private Substitute? GetSubstitute(OperationAnalysisContext operationAnalysisContext, IInvocationOperation invocationOperation)
    {
        var substituteOperation = _substitutionOperationFinder
            .Find(operationAnalysisContext.Compilation, invocationOperation).FirstOrDefault();

        if (substituteOperation == null)
        {
            return null;
        }

        return Substitute.TryCreate(substituteOperation, invocationOperation);
    }

    private IndexerInfo GetIndexerInfo(IOperation indexerOperation)
    {
        ISymbol? info = indexerOperation switch
        {
            IArrayElementReferenceOperation x => x.Type,
            _ => null
        };

        var symbol = info as IMethodSymbol;
        var verifyIndexerCast = symbol == null || symbol.Name != MetadataNames.CallInfoArgTypesMethod;
        var verifyAssignment = symbol == null;

        var indexerInfo = new IndexerInfo(verifyIndexerCast, verifyAssignment);
        return indexerInfo;
    }

    // See https://github.com/nsubstitute/NSubstitute/blob/26d0b0b880c623ef8cae8a0a71360ae2a9982f53/src/NSubstitute/Core/CallInfo.cs#L70
    // for the logic behind it
    private int GetMatchingParametersCount(Compilation compilation, Substitute substitute, ITypeSymbol typeSymbol)
    {
        var declaringTypeMatchCount =
            substitute.CallArguments.Count(type => type.ParameterTypeSymbol.Equals(typeSymbol));

        if (declaringTypeMatchCount > 0)
        {
            return declaringTypeMatchCount;
        }

        return substitute.CallArguments.Count(type => IsAssignableTo(compilation, type.ArgumentTypeSymbol, typeSymbol));
    }

    private struct IndexerInfo
    {
        public bool VerifyIndexerCast { get; }

        public bool VerifyAssignment { get; }

        public IndexerInfo(bool verifyIndexerCast, bool verifyAssignment)
        {
            VerifyIndexerCast = verifyIndexerCast;
            VerifyAssignment = verifyAssignment;
        }
    }
}