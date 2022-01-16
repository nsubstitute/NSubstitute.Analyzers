using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractCallInfoAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TExpressionSyntax, TIndexerExpressionSyntax> : AbstractDiagnosticAnalyzer
    where TInvocationExpressionSyntax : SyntaxNode
    where TExpressionSyntax : SyntaxNode
    where TIndexerExpressionSyntax : SyntaxNode
    where TSyntaxKind : struct
{
    private readonly ICallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax> _callInfoFinder;
    private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;
    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    protected AbstractCallInfoAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ICallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax> callInfoFinder,
        ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder)
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

    protected abstract IEnumerable<TExpressionSyntax> GetArgumentExpressions(TInvocationExpressionSyntax invocationExpressionSyntax);

    protected abstract SyntaxNode GetCastTypeExpression(TIndexerExpressionSyntax indexerExpressionSyntax);

    protected abstract SyntaxNode GetAssignmentExpression(TIndexerExpressionSyntax indexerExpressionSyntax);

    protected abstract ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

    protected abstract int? GetArgAtPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TInvocationExpressionSyntax invocationExpressionSyntax);

    protected abstract int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

    protected abstract bool CanCast(Compilation compilation, ITypeSymbol sourceSymbol, ITypeSymbol destinationSymbol);

    protected abstract bool IsAssignableTo(Compilation compilation, ITypeSymbol fromSymbol, ITypeSymbol toSymbol);

    private bool SupportsCallInfo(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax syntax, IMethodSymbol methodSymbol)
    {
        if (methodSymbol.IsCallInfoSupportingMethod() == false)
        {
            return false;
        }

        var allArguments = GetArgumentExpressions(syntax);
        IEnumerable<TExpressionSyntax> argumentsForAnalysis;
        if (methodSymbol.MethodKind == MethodKind.ReducedExtension)
            argumentsForAnalysis = allArguments;
        else if (methodSymbol.IsExtensionMethod)
            argumentsForAnalysis = allArguments.Skip(1);
        else
            argumentsForAnalysis = allArguments;

        // perf - dont use linq in hotpath
        foreach (var arg in argumentsForAnalysis)
        {
            if (syntaxNodeContext.SemanticModel.GetTypeInfo(arg).IsCallInfoDelegate(syntaxNodeContext.SemanticModel))
            {
                return true;
            }
        }

        return false;
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
        var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

        if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
        {
            return;
        }

        var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;
        if (SupportsCallInfo(syntaxNodeContext, invocationExpression, methodSymbol) == false)
        {
            return;
        }

        var substituteCallParameters = GetSubstituteCallArgumentOperations(syntaxNodeContext, methodSymbol, invocationExpression);

        if (substituteCallParameters == null)
        {
            return;
        }

        foreach (var argumentExpressionSyntax in GetArgumentExpressions(invocationExpression))
        {
            var callInfoContext = _callInfoFinder.GetCallInfoContext(syntaxNodeContext.SemanticModel, argumentExpressionSyntax);

            AnalyzeArgAtInvocations(syntaxNodeContext, callInfoContext, substituteCallParameters);

            AnalyzeArgInvocations(syntaxNodeContext, callInfoContext, substituteCallParameters);

            AnalyzeIndexerInvocations(syntaxNodeContext, callInfoContext, substituteCallParameters);
        }
    }

    private void AnalyzeIndexerInvocations(SyntaxNodeAnalysisContext syntaxNodeContext, CallInfoContext<TInvocationExpressionSyntax, TIndexerExpressionSyntax> callInfoContext, IList<IArgumentOperation> substituteCallParameters)
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

    private void AnalyzeArgAtInvocations(SyntaxNodeAnalysisContext syntaxNodeContext, CallInfoContext<TInvocationExpressionSyntax, TIndexerExpressionSyntax> callInfoContext, IList<IArgumentOperation> substituteCallParameters)
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
                    IsAssignableTo(syntaxNodeContext.Compilation, substituteCallParameters[position.Value].GetArgumentOperationActualTypeSymbol(), argAtMethodSymbol.TypeArguments.First()) == false)
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

    private void AnalyzeArgInvocations(SyntaxNodeAnalysisContext syntaxNodeContext, CallInfoContext<TInvocationExpressionSyntax, TIndexerExpressionSyntax> callInfoContext, IList<IArgumentOperation> substituteCallParameters)
    {
        foreach (var argInvocation in callInfoContext.ArgInvocations)
        {
            var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(argInvocation);
            if (symbolInfo.Symbol != null && symbolInfo.Symbol is IMethodSymbol argMethodSymbol)
            {
                var typeSymbol = argMethodSymbol.TypeArguments.First();
                var parameterCount = GetMatchingParametersCount(syntaxNodeContext, substituteCallParameters, typeSymbol);
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

    private bool AnalyzeArgumentAccess(SyntaxNodeAnalysisContext syntaxNodeContext, IList<IArgumentOperation> substituteCallParameters, TIndexerExpressionSyntax indexer, int? position)
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

    private bool AnalyzeCast(SyntaxNodeAnalysisContext syntaxNodeContext, IList<IArgumentOperation> substituteCallParameters, TIndexerExpressionSyntax indexer, in IndexerInfo indexerInfo, int? position)
    {
        var castTypeExpression = GetCastTypeExpression(indexer);
        if (position.HasValue && indexerInfo.VerifyIndexerCast && castTypeExpression != null)
        {
            var typeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(castTypeExpression);
            if (typeInfo.Type != null && CanCast(syntaxNodeContext.Compilation, substituteCallParameters[position.Value].GetArgumentOperationActualTypeSymbol(), typeInfo.Type) == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
                    indexer.GetLocation(),
                    position.Value,
                    typeInfo.Type);
                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }
        }

        return false;
    }

    private bool AnalyzeAssignment(SyntaxNodeAnalysisContext syntaxNodeContext, IList<IArgumentOperation> substituteCallParameters, TIndexerExpressionSyntax indexer, in IndexerInfo indexerInfo, int? position)
    {
        var assignmentExpressionSyntax = GetAssignmentExpression(indexer);
        if (indexerInfo.VerifyAssignment && assignmentExpressionSyntax != null && position.HasValue && position.Value < substituteCallParameters.Count)
        {
            var parameterSymbol = substituteCallParameters[position.Value];
            if (parameterSymbol.Parameter.RefKind != RefKind.Out && parameterSymbol.Parameter.RefKind != RefKind.Ref)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoArgumentIsNotOutOrRef,
                    indexer.GetLocation(),
                    position.Value,
                    parameterSymbol.GetArgumentOperationDeclaredTypeSymbol());
                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }

            var typeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(assignmentExpressionSyntax);
            var typeSymbol = substituteCallParameters[position.Value].GetArgumentOperationDeclaredTypeSymbol();
            if (typeInfo.Type != null && IsAssignableTo(syntaxNodeContext.Compilation, typeInfo.Type, typeSymbol) == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.CallInfoArgumentSetWithIncompatibleValue,
                    indexer.GetLocation(),
                    typeInfo.Type,
                    position.Value,
                    typeSymbol);
                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }
        }

        return false;
    }

    private IList<IArgumentOperation> GetSubstituteCallArgumentOperations(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, TInvocationExpressionSyntax invocationExpression)
    {
        var parentMethodCallSyntax = _substitutionNodeFinder.Find(syntaxNodeContext, invocationExpression, methodSymbol).FirstOrDefault();

        if (parentMethodCallSyntax == null)
        {
            return null;
        }

        var operation = syntaxNodeContext.SemanticModel.GetOperation(parentMethodCallSyntax);
        IEnumerable<IArgumentOperation> argumentOperations;
        switch (operation)
        {
            case IInvocationOperation substituteMethodSymbol:
                argumentOperations = substituteMethodSymbol.Arguments;
                break;
            case IPropertyReferenceOperation propertySymbol:
                argumentOperations = propertySymbol.Arguments;
                break;
            default:
                return null;
        }

        return argumentOperations.OrderBy(argOperation => argOperation.Parameter.Ordinal).ToList();
    }

    private IndexerInfo GetIndexerInfo(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax)
    {
        var info = GetIndexerSymbol(syntaxNodeAnalysisContext, indexerExpressionSyntax);
        var symbol = info as IMethodSymbol;
        var verifyIndexerCast = symbol == null || symbol.Name != MetadataNames.CallInfoArgTypesMethod;
        var verifyAssignment = symbol == null;

        var indexerInfo = new IndexerInfo(verifyIndexerCast, verifyAssignment);
        return indexerInfo;
    }

    // See https://github.com/nsubstitute/NSubstitute/blob/26d0b0b880c623ef8cae8a0a71360ae2a9982f53/src/NSubstitute/Core/CallInfo.cs#L70
    // for the logic behind it
    private int GetMatchingParametersCount(SyntaxNodeAnalysisContext syntaxNodeContext, IList<IArgumentOperation> substituteCallParameters, ITypeSymbol typeSymbol)
    {
        var declaringTypeMatchCount =
            substituteCallParameters.Count(param => param.GetArgumentOperationDeclaredTypeSymbol() == typeSymbol);

        if (declaringTypeMatchCount > 0)
        {
            return declaringTypeMatchCount;
        }

        return substituteCallParameters.Count(param => IsAssignableTo(syntaxNodeContext.Compilation, param.GetArgumentOperationActualTypeSymbol(), typeSymbol));
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