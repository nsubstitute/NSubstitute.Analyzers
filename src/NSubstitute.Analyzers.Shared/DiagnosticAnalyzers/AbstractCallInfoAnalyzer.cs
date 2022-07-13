using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractCallInfoAnalyzer<TSyntaxKind> : AbstractDiagnosticAnalyzer
    where TSyntaxKind : struct
{
    private readonly ICallInfoFinder _callInfoFinder;
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;
    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

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
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

    protected abstract bool CanCast(Compilation compilation, ITypeSymbol sourceSymbol, ITypeSymbol destinationSymbol);

    protected abstract bool IsAssignableTo(Compilation compilation, ITypeSymbol fromSymbol, ITypeSymbol toSymbol);

    protected int? GetArgAtPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode invocationExpressionSyntax)
    {
        var operation = syntaxNodeAnalysisContext.SemanticModel.GetOperation(invocationExpressionSyntax);

        var literal = operation switch
        {
            IInvocationOperation invocationOperation =>
                invocationOperation.Arguments.First().Value as ILiteralOperation,
            IArrayElementReferenceOperation arrayElementReferenceOperation => arrayElementReferenceOperation.Indices
                .First() as ILiteralOperation,
            IPropertyReferenceOperation propertyReferenceOperation =>
                propertyReferenceOperation.Arguments.First().Value as ILiteralOperation,
            _ => null
        };

        if (literal == null || literal.ConstantValue.HasValue == false)
        {
            return null;
        }

        return (int)literal.ConstantValue.Value;
    }

    protected int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode indexerExpressionSyntax)
    {
        var operation = syntaxNodeAnalysisContext.SemanticModel.GetOperation(indexerExpressionSyntax);
        return operation.GetIndexerPosition();
    }

    private bool SupportsCallInfo(SyntaxNodeAnalysisContext syntaxNodeContext, IInvocationOperation invocationOperation)
    {
        if (invocationOperation.TargetMethod.IsCallInfoSupportingMethod() == false)
        {
            return false;
        }

        // perf - dont use linq in hotpath
        foreach (var arg in invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument())
        {
            if (arg.GetTypeSymbol().IsCallInfoDelegate(syntaxNodeContext.SemanticModel))
            {
                return true;
            }
        }

        return false;
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        if (!(syntaxNodeContext.SemanticModel.GetOperation(syntaxNodeContext.Node) is IInvocationOperation
                invocationOperation))
        {
            return;
        }

        if (SupportsCallInfo(syntaxNodeContext, invocationOperation) == false)
        {
            return;
        }

        var substituteCallParameters = GetSubstituteCallArgumentOperations(syntaxNodeContext, invocationOperation);

        if (substituteCallParameters == null)
        {
            return;
        }

        foreach (var argumentExpressionSyntax in
                 invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument())
        {
            var callInfoContext =
                _callInfoFinder.GetCallInfoContext(argumentExpressionSyntax);

            AnalyzeArgAtInvocations(syntaxNodeContext, callInfoContext, substituteCallParameters);

            AnalyzeArgInvocations(syntaxNodeContext, callInfoContext, substituteCallParameters);

            AnalyzeIndexerInvocations(syntaxNodeContext, callInfoContext, substituteCallParameters);
        }
    }

    private void AnalyzeIndexerInvocations(SyntaxNodeAnalysisContext syntaxNodeContext, CallInfoContext callInfoContext, IReadOnlyList<IArgumentOperation> substituteCallParameters)
    {
        foreach (var indexer in callInfoContext.IndexerAccesses)
        {
            var indexerInfo = GetIndexerInfo(syntaxNodeContext, indexer);

            var position = GetIndexerPosition(syntaxNodeContext, indexer);

            if (AnalyzeArgumentAccess(syntaxNodeContext, substituteCallParameters, indexer, position))
            {
                continue;
            }

            if (AnalyzeCast(syntaxNodeContext, substituteCallParameters, indexer, in indexerInfo, position))
            {
                continue;
            }

            AnalyzeAssignment(syntaxNodeContext, substituteCallParameters, indexer, indexerInfo, position);
        }
    }

    private void AnalyzeArgAtInvocations(SyntaxNodeAnalysisContext syntaxNodeContext, CallInfoContext callInfoContext, IReadOnlyList<IArgumentOperation> substituteCallParameters)
    {
        foreach (var argAtInvocation in callInfoContext.ArgAtInvocations)
        {
            var position = GetArgAtPosition(syntaxNodeContext, argAtInvocation);
            if (position.HasValue)
            {
                if (position.Value > substituteCallParameters.Count - 1)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
                        argAtInvocation.GetLocation(),
                        position);

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                    continue;
                }

                var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(argAtInvocation);
                if (symbolInfo.Symbol != null &&
                    symbolInfo.Symbol is IMethodSymbol argAtMethodSymbol &&
                    IsAssignableTo(
                        syntaxNodeContext.Compilation,
                        substituteCallParameters[position.Value].GetTypeSymbol(),
                        argAtMethodSymbol.TypeArguments.First()) == false)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
                        argAtInvocation.GetLocation(),
                        position,
                        argAtMethodSymbol.TypeArguments.First());

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    private void AnalyzeArgInvocations(SyntaxNodeAnalysisContext syntaxNodeContext, CallInfoContext callInfoContext, IReadOnlyList<IArgumentOperation> substituteCallParameters)
    {
        foreach (var argInvocation in callInfoContext.ArgInvocations)
        {
            var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(argInvocation);
            if (symbolInfo.Symbol != null && symbolInfo.Symbol is IMethodSymbol argMethodSymbol)
            {
                var typeSymbol = argMethodSymbol.TypeArguments.First();
                var parameterCount =
                    GetMatchingParametersCount(syntaxNodeContext, substituteCallParameters, typeSymbol);
                if (parameterCount == 0)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.CallInfoCouldNotFindArgumentToThisCall,
                        argInvocation.GetLocation(),
                        typeSymbol);

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                    continue;
                }

                if (parameterCount > 1)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.CallInfoMoreThanOneArgumentOfType,
                        argInvocation.GetLocation(),
                        typeSymbol);

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    private bool AnalyzeArgumentAccess(SyntaxNodeAnalysisContext syntaxNodeContext, IReadOnlyList<IArgumentOperation> substituteCallParameters, SyntaxNode indexer, int? position)
    {
        if (position.HasValue && position.Value > substituteCallParameters.Count - 1)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
                indexer.GetLocation(),
                position.Value);

            syntaxNodeContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeCast(SyntaxNodeAnalysisContext syntaxNodeContext, IReadOnlyList<IArgumentOperation> substituteCallParameters, SyntaxNode indexer, in IndexerInfo indexerInfo, int? position)
    {
        if (!position.HasValue || !indexerInfo.VerifyIndexerCast)
        {
            return false;
        }

        if (!(syntaxNodeContext.SemanticModel.GetOperation(indexer).Parent is IConversionOperation conversionOperation))
        {
            return false;
        }

        var type = conversionOperation.Type;
        if (type != null && CanCast(syntaxNodeContext.Compilation, substituteCallParameters[position.Value].GetTypeSymbol(), type) == false)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
                indexer.GetLocation(),
                position.Value,
                type);
            syntaxNodeContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeAssignment(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        IReadOnlyList<IArgumentOperation> substituteCallParameters,
        SyntaxNode indexer,
        in IndexerInfo indexerInfo,
        int? position)
    {
        if (!indexerInfo.VerifyAssignment || !position.HasValue || position.Value >= substituteCallParameters.Count)
        {
            return false;
        }

        if (syntaxNodeContext.SemanticModel.GetOperation(indexer) is IPropertyReferenceOperation referenceOperation &&
            referenceOperation.Parent is ISimpleAssignmentOperation simpleAssignmentOperation)
        {
            var parameterSymbol = substituteCallParameters[position.Value];
            if (parameterSymbol.Parameter.RefKind != RefKind.Out &&
                parameterSymbol.Parameter.RefKind != RefKind.Ref)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoArgumentIsNotOutOrRef,
                    indexer.GetLocation(),
                    position.Value,
                    parameterSymbol.GetArgumentOperationDeclaredTypeSymbol());
                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }

            var assignmentType = simpleAssignmentOperation.GetTypeSymbol();
            var typeSymbol = substituteCallParameters[position.Value].GetArgumentOperationDeclaredTypeSymbol();
            if (assignmentType != null &&
                IsAssignableTo(syntaxNodeContext.Compilation, assignmentType, typeSymbol) == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoArgumentSetWithIncompatibleValue,
                    indexer.GetLocation(),
                    assignmentType,
                    position.Value,
                    typeSymbol);
                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }
        }

        return false;
    }

    private IReadOnlyList<IArgumentOperation> GetSubstituteCallArgumentOperations(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        IInvocationOperation invocationOperation)
    {
        var parentMethodCallSyntax =
            _substitutionNodeFinder.Find(syntaxNodeContext, invocationOperation).FirstOrDefault();

        if (parentMethodCallSyntax == null)
        {
            return null;
        }

        var operation = syntaxNodeContext.SemanticModel.GetOperation(parentMethodCallSyntax);
        IEnumerable<IArgumentOperation> argumentOperations = operation switch
        {
            IInvocationOperation substituteMethodSymbol => substituteMethodSymbol.Arguments,
            IPropertyReferenceOperation propertySymbol => propertySymbol.Arguments,
            _ => null
        };

        return argumentOperations?.OrderBy(argOperation => argOperation.Parameter.Ordinal).ToList();
    }

    private IndexerInfo GetIndexerInfo(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode indexerExpressionSyntax)
    {
        var operation = syntaxNodeAnalysisContext.SemanticModel.GetOperation(indexerExpressionSyntax);
        ISymbol info = operation switch
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
    private int GetMatchingParametersCount(SyntaxNodeAnalysisContext syntaxNodeContext, IReadOnlyList<IArgumentOperation> substituteCallParameters, ITypeSymbol typeSymbol)
    {
        var declaringTypeMatchCount =
            substituteCallParameters.Count(param => param.GetArgumentOperationDeclaredTypeSymbol() == typeSymbol);

        if (declaringTypeMatchCount > 0)
        {
            return declaringTypeMatchCount;
        }

        return substituteCallParameters.Count(param =>
            IsAssignableTo(syntaxNodeContext.Compilation, param.GetTypeSymbol(), typeSymbol));
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