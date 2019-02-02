using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractCallInfoAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TExpressionSyntax, TIndexerExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TExpressionSyntax : SyntaxNode
        where TIndexerExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        private readonly Lazy<AbstractCallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax>> _callInfoFinderProxy;

        private AbstractCallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax> CallInfoFinder => _callInfoFinderProxy.Value;

        protected AbstractCallInfoAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            _callInfoFinderProxy = new Lazy<AbstractCallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax>>(GetCallInfoFinder);
        }

        private static readonly ImmutableDictionary<string, string> MethodNames = new Dictionary<string, string>()
        {
            [MetadataNames.NSubstituteReturnsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsForAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteThrowsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName,
            [MetadataNames.NSubstituteThrowsForAnyArgsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName,
            [MetadataNames.NSubstituteAndDoesMethod] = MetadataNames.NSubstituteConfiguredCallFullTypeName
        }.ToImmutableDictionary();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
            DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
            DiagnosticDescriptorsProvider.CallInfoCouldNotFindArgumentToThisCall,
            DiagnosticDescriptorsProvider.CallInfoMoreThanOneArgumentOfType,
            DiagnosticDescriptorsProvider.CallInfoArgumentSetWithIncompatibleValue,
            DiagnosticDescriptorsProvider.CallInfoArgumentIsNotOutOrRef);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, InvocationExpressionKind);
        }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected abstract SyntaxNode GetSubstituteCall(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract IEnumerable<TExpressionSyntax> GetArgumentExpressions(TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract AbstractCallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax> GetCallInfoFinder();

        protected abstract SyntaxNode GetCastTypeExpression(TIndexerExpressionSyntax indexerExpressionSyntax);

        protected abstract SyntaxNode GetAssignmentExpression(TIndexerExpressionSyntax indexerExpressionSyntax);

        protected abstract ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

        protected abstract int? GetArgAtPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

        protected abstract bool CanCast(Compilation compilation, ITypeSymbol sourceSymbol, ITypeSymbol destinationSymbol);

        protected abstract bool IsAssignableTo(Compilation compilation, ITypeSymbol fromSymbol, ITypeSymbol toSymbol);

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

            var substituteCallParameters = GetSubstituteCallParameters(syntaxNodeContext, methodSymbol, invocationExpression);

            if (substituteCallParameters == null)
            {
                return;
            }

            foreach (var argumentExpressionSyntax in GetArgumentExpressions(invocationExpression))
            {
                var callInfoContext = CallInfoFinder.GetCallInfoContext(syntaxNodeContext.SemanticModel, argumentExpressionSyntax);

                AnalyzeArgAtInvocations(syntaxNodeContext, callInfoContext, substituteCallParameters);

                AnalyzeArgInvocations(syntaxNodeContext, callInfoContext, substituteCallParameters);

                AnalyzeIndexerInvocations(syntaxNodeContext, callInfoContext, substituteCallParameters);
            }
        }

        private void AnalyzeIndexerInvocations(SyntaxNodeAnalysisContext syntaxNodeContext, CallInfoContext<TInvocationExpressionSyntax, TIndexerExpressionSyntax> callInfoContext, IList<IParameterSymbol> substituteCallParameters)
        {
            foreach (var indexer in callInfoContext.IndexerAccesses)
            {
                var indexerInfo = GetIndexerInfo(syntaxNodeContext, indexer);

                var position = GetIndexerPosition(syntaxNodeContext, indexer);

                if (AnalyzeArgumentAccess(syntaxNodeContext, substituteCallParameters, indexer, position))
                {
                    continue;
                }

                if (AnalyzeCast(syntaxNodeContext, substituteCallParameters, indexer, indexerInfo, position))
                {
                    continue;
                }

                AnalyzeAssignment(syntaxNodeContext, substituteCallParameters, indexer, indexerInfo, position);
            }
        }

        private void AnalyzeArgAtInvocations(SyntaxNodeAnalysisContext syntaxNodeContext, CallInfoContext<TInvocationExpressionSyntax, TIndexerExpressionSyntax> callInfoContext, IList<IParameterSymbol> substituteCallParameters)
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
                        IsAssignableTo(syntaxNodeContext.Compilation, substituteCallParameters[position.Value].Type, argAtMethodSymbol.TypeArguments.First()) == false)
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

        private void AnalyzeArgInvocations(SyntaxNodeAnalysisContext syntaxNodeContext, CallInfoContext<TInvocationExpressionSyntax, TIndexerExpressionSyntax> callInfoContext, IList<IParameterSymbol> substituteCallParameters)
        {
            foreach (var argInvocation in callInfoContext.ArgInvocations)
            {
                var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(argInvocation);
                if (symbolInfo.Symbol != null && symbolInfo.Symbol is IMethodSymbol argMethodSymbol)
                {
                    var typeSymbol = argMethodSymbol.TypeArguments.First();
                    var parameterCount = substituteCallParameters.Count(param => IsAssignableTo(syntaxNodeContext.Compilation, param.Type, typeSymbol));
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

        private bool AnalyzeArgumentAccess(SyntaxNodeAnalysisContext syntaxNodeContext, IList<IParameterSymbol> substituteCallParameters, TIndexerExpressionSyntax indexer, int? position)
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

        private bool AnalyzeCast(SyntaxNodeAnalysisContext syntaxNodeContext, IList<IParameterSymbol> substituteCallParameters, TIndexerExpressionSyntax indexer, IndexerInfo indexerInfo, int? position)
        {
            var castTypeExpression = GetCastTypeExpression(indexer);
            if (position.HasValue && indexerInfo.VerifyIndexerCast && castTypeExpression != null)
            {
                var typeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(castTypeExpression);
                if (typeInfo.Type != null && CanCast(syntaxNodeContext.Compilation, substituteCallParameters[position.Value].Type, typeInfo.Type) == false)
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

        private bool AnalyzeAssignment(SyntaxNodeAnalysisContext syntaxNodeContext, IList<IParameterSymbol> substituteCallParameters, TIndexerExpressionSyntax indexer, IndexerInfo indexerInfo, int? position)
        {
            var assignmentExpressionSyntax = GetAssignmentExpression(indexer);
            if (indexerInfo.VerifyAssignment && assignmentExpressionSyntax != null && position.HasValue && position.Value < substituteCallParameters.Count)
            {
                var parameterSymbol = substituteCallParameters[position.Value];
                if (parameterSymbol.RefKind != RefKind.Out && parameterSymbol.RefKind != RefKind.Ref)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.CallInfoArgumentIsNotOutOrRef,
                        indexer.GetLocation(),
                        position.Value,
                        parameterSymbol.Type);
                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                    return true;
                }

                var typeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(assignmentExpressionSyntax);
                if (typeInfo.Type != null && IsAssignableTo(syntaxNodeContext.Compilation, typeInfo.Type, substituteCallParameters[position.Value].Type) == false)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.CallInfoArgumentSetWithIncompatibleValue,
                        indexer.GetLocation(),
                        typeInfo.Type,
                        position.Value,
                        substituteCallParameters[position.Value].Type);
                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                    return true;
                }
            }

            return false;
        }

        private bool SupportsCallInfo(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax syntax, IMethodSymbol methodSymbol)
        {
            // TODO Simplify
            var allArguments = GetArgumentExpressions(syntax);
            var argumentsForAnalysis = methodSymbol.MethodKind == MethodKind.ReducedExtension
                ? allArguments
                : methodSymbol.IsExtensionMethod ? allArguments.Skip(1) : allArguments;

            if (MethodNames.TryGetValue(methodSymbol.Name, out var typeName) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            var supportsCallInfo =
                symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                symbol.Symbol?.ContainingType?.ToString().Equals(typeName, StringComparison.OrdinalIgnoreCase) == true;

            return supportsCallInfo && argumentsForAnalysis.Any(arg => syntaxNodeContext.SemanticModel.GetTypeInfo(arg).IsCallInfoDelegate(syntaxNodeContext.SemanticModel));
        }

        private IList<IParameterSymbol> GetSubstituteCallParameters(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, TInvocationExpressionSyntax invocationExpression)
        {
            var parentMethodCallSyntax = GetSubstituteCall(syntaxNodeContext, methodSymbol, invocationExpression);
            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(parentMethodCallSyntax).Symbol;

            switch (symbol)
            {
                case IMethodSymbol substituteMethodSymbol:
                    return substituteMethodSymbol.Parameters;
                case IPropertySymbol propertySymbol:
                    return propertySymbol.Parameters;
                default:
                    return null;
            }
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
}