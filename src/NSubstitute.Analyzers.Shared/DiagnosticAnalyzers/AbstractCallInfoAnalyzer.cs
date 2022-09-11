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
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;
    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    protected AbstractCallInfoAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ICallInfoFinder callInfoFinder,
        ISubstitutionNodeFinder substitutionNodeFinder)
        : base(diagnosticDescriptorsProvider)
    {
        _callInfoFinder = callInfoFinder;
        _substitutionNodeFinder = substitutionNodeFinder;
        _analyzeInvocationAction = AnalyzeInvocation;

        SupportedDiagnostics = ImmutableArray.Create(
            DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
            DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
            DiagnosticDescriptorsProvider.CallInfoCouldNotFindArgumentToThisCall,
            DiagnosticDescriptorsProvider.CallInfoMoreThanOneArgumentOfType,
            DiagnosticDescriptorsProvider.CallInfoArgumentSetWithIncompatibleValue,
            DiagnosticDescriptorsProvider.CallInfoArgumentIsNotOutOrRef);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected override void InitializeAnalyzer(AnalysisContext context)
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

        var substituteCallParameters = GetSubstituteCallArgumentOperations(operationAnalysisContext, invocationOperation);

        if (substituteCallParameters == null)
        {
            return;
        }

        foreach (var argumentExpressionSyntax in
                 invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument())
        {
            var callInfoContext = _callInfoFinder.GetCallInfoContext(argumentExpressionSyntax);

            AnalyzeArgAtInvocations(operationAnalysisContext, callInfoContext, substituteCallParameters);

            AnalyzeArgInvocations(operationAnalysisContext, callInfoContext, substituteCallParameters);

            AnalyzeIndexerInvocations(operationAnalysisContext, callInfoContext, substituteCallParameters);
        }
    }

    private void AnalyzeIndexerInvocations(OperationAnalysisContext operationAnalysisContext, CallInfoContext callInfoContext, IReadOnlyList<IArgumentOperation> substituteCallParameters)
    {
        foreach (var indexer in callInfoContext.IndexerAccessesOperations)
        {
            var indexerInfo = GetIndexerInfo(indexer);

            var position = indexer.GetIndexerPosition();

            if (AnalyzeArgumentAccess(operationAnalysisContext, substituteCallParameters, indexer, position))
            {
                continue;
            }

            if (AnalyzeCast(operationAnalysisContext, substituteCallParameters, indexer, in indexerInfo, position))
            {
                continue;
            }

            AnalyzeAssignment(operationAnalysisContext, substituteCallParameters, indexer, indexerInfo, position);
        }
    }

    private void AnalyzeArgAtInvocations(OperationAnalysisContext operationAnalysisContext, CallInfoContext callInfoContext, IReadOnlyList<IArgumentOperation> substituteCallParameters)
    {
        foreach (var argAtInvocation in callInfoContext.ArgAtInvocationsOperations)
        {
            var position = argAtInvocation.GetIndexerPosition();
            if (position.HasValue)
            {
                if (position.Value > substituteCallParameters.Count - 1)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
                        argAtInvocation.Syntax.GetLocation(),
                        position);

                    operationAnalysisContext.ReportDiagnostic(diagnostic);
                    continue;
                }

                if (IsAssignableTo(
                        operationAnalysisContext.Compilation,
                        substituteCallParameters[position.Value].GetTypeSymbol(),
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
    }

    private void AnalyzeArgInvocations(OperationAnalysisContext operationAnalysisContext, CallInfoContext callInfoContext, IReadOnlyList<IArgumentOperation> substituteCallParameters)
    {
        foreach (var argInvocationOperation in callInfoContext.ArgInvocationsOperations)
        {
            var typeSymbol = argInvocationOperation.TargetMethod.TypeArguments.First();
            var parameterCount =
                GetMatchingParametersCount(operationAnalysisContext.Compilation, substituteCallParameters, typeSymbol);
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
        IReadOnlyList<IArgumentOperation> substituteCallParameters,
        IOperation indexerOperation,
        int? position)
    {
        if (position.HasValue && position.Value > substituteCallParameters.Count - 1)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
                indexerOperation.Syntax.GetLocation(),
                position.Value);

            syntaxNodeContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeCast(OperationAnalysisContext operationAnalysisContext, IReadOnlyList<IArgumentOperation> substituteCallParameters, IOperation indexer, in IndexerInfo indexerInfo, int? position)
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
        if (type != null && CanCast(operationAnalysisContext.Compilation, substituteCallParameters[position.Value].GetTypeSymbol(), type) == false)
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
        IReadOnlyList<IArgumentOperation> substituteCallParameters,
        IOperation indexerOperation,
        in IndexerInfo indexerInfo,
        int? position)
    {
        if (!indexerInfo.VerifyAssignment || !position.HasValue || position.Value >= substituteCallParameters.Count)
        {
            return false;
        }

        if (indexerOperation is IPropertyReferenceOperation { Parent: ISimpleAssignmentOperation simpleAssignmentOperation })
        {
            var parameterSymbol = substituteCallParameters[position.Value];
            if (parameterSymbol.Parameter.RefKind != RefKind.Out &&
                parameterSymbol.Parameter.RefKind != RefKind.Ref)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoArgumentIsNotOutOrRef,
                    indexerOperation.Syntax.GetLocation(),
                    position.Value,
                    parameterSymbol.GetArgumentOperationDeclaredTypeSymbol());
                operationAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }

            var assignmentType = simpleAssignmentOperation.GetTypeSymbol();
            var typeSymbol = substituteCallParameters[position.Value].GetArgumentOperationDeclaredTypeSymbol();
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

    private IReadOnlyList<IArgumentOperation> GetSubstituteCallArgumentOperations(OperationAnalysisContext operationAnalysisContext, IInvocationOperation invocationOperation)
    {
        var substituteOperation = _substitutionNodeFinder
            .Find(operationAnalysisContext.Compilation, invocationOperation).FirstOrDefault();

        if (substituteOperation == null)
        {
            return null;
        }

        var argumentOperations = GetArgumentOperations(substituteOperation);

        return argumentOperations?.OrderBy(argOperation => argOperation.Parameter.Ordinal).ToList();
    }

    private static IEnumerable<IArgumentOperation> GetArgumentOperations(IOperation substituteOperation)
    {
        return substituteOperation switch
        {
            IInvocationOperation substituteMethodSymbol => substituteMethodSymbol.Arguments,
            IPropertyReferenceOperation propertySymbol => propertySymbol.Arguments,
            IConversionOperation conversionOperation => GetArgumentOperations(conversionOperation.Operand),
            _ => null
        };
    }

    private IndexerInfo GetIndexerInfo(IOperation indexerOperation)
    {
        ISymbol info = indexerOperation switch
        {
            IInvocationOperation inv => inv.TargetMethod,
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
    private int GetMatchingParametersCount(Compilation compilation, IReadOnlyList<IArgumentOperation> substituteCallParameters, ITypeSymbol typeSymbol)
    {
        var declaringTypeMatchCount =
            substituteCallParameters.Count(param => param.GetArgumentOperationDeclaredTypeSymbol().Equals(typeSymbol));

        if (declaringTypeMatchCount > 0)
        {
            return declaringTypeMatchCount;
        }

        return substituteCallParameters.Count(param =>
            IsAssignableTo(compilation, param.GetTypeSymbol(), typeSymbol));
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