using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractCallInfoAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TExpressionSyntax, TIndexerExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TExpressionSyntax : SyntaxNode
        where TIndexerExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        protected AbstractCallInfoAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
        }

        private static readonly ImmutableDictionary<string, string> MethodNames = new Dictionary<string, string>()
        {
            [MetadataNames.NSubstituteReturnsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsForAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteThrowsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName,
            [MetadataNames.NSubstituteThrowsForAnyArgsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName
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

        protected abstract SyntaxNode GetSubstituteCall(IMethodSymbol methodSymbol, TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract IEnumerable<TExpressionSyntax> GetArgumentExpressions(TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract AbstractCallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax> GetCallInfoFinder();

        protected abstract SyntaxNode GetCastTypeExpression(TIndexerExpressionSyntax indexerExpressionSyntax);

        protected abstract SyntaxNode GetAssignmentExpression(TIndexerExpressionSyntax indexerExpressionSyntax);

        protected abstract ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

        protected abstract int? GetArgAtPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

        private IndexerInfo GetIndexerInfo(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax)
        {
            var info = GetIndexerSymbol(syntaxNodeAnalysisContext, indexerExpressionSyntax);
            var symbol = info as IMethodSymbol;
            var verifyIndexerCast = symbol == null || symbol.Name != MetadataNames.CallInfoArgTypesMethod;
            var verifyAssignment = symbol == null;

            var indexerInfo = new IndexerInfo(verifyIndexerCast, verifyAssignment);
            return indexerInfo;
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

            var substituteCallParameters = GetSubstituteCallParameters(syntaxNodeContext, methodSymbol, invocationExpression);

            if (substituteCallParameters == null)
            {
                return;
            }

            foreach (var argumentExpressionSyntax in GetArgumentExpressions(invocationExpression))
            {
                var finder = GetCallInfoFinder();
                var callInfoContext = finder.GetCallInfoContext(syntaxNodeContext.SemanticModel, argumentExpressionSyntax);
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
                            Equals(substituteCallParameters[position.Value].Type, argAtMethodSymbol.TypeArguments.First()) == false)
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

                foreach (var argInvocation in callInfoContext.ArgInvocations)
                {
                    var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(argInvocation);
                    if (symbolInfo.Symbol != null && symbolInfo.Symbol is IMethodSymbol argMethodSymbol)
                    {
                        var typeSymbol = argMethodSymbol.TypeArguments.First();
                        var parameterCount = substituteCallParameters.Count(param => Equals(param.Type, typeSymbol));
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

                foreach (var indexer in callInfoContext.IndexerAccesses)
                {
                    var indexerInfo = GetIndexerInfo(syntaxNodeContext, indexer);

                    var position = GetIndexerPosition(syntaxNodeContext, indexer);
                    if (position.HasValue && position.Value > substituteCallParameters.Count - 1)
                    {
                        var diagnostic = Diagnostic.Create(
                            DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
                            indexer.GetLocation(),
                            position.Value);

                        syntaxNodeContext.ReportDiagnostic(diagnostic);
                        continue;
                    }

                    var castTypeExpression = GetCastTypeExpression(indexer);
                    if (indexerInfo.VerifyIndexerCast && castTypeExpression != null)
                    {
                        var typeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(castTypeExpression);
                        if (typeInfo.Type != null && !Equals(typeInfo.Type, substituteCallParameters[position.Value].Type))
                        {
                            var diagnostic = Diagnostic.Create(
                                DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
                                indexer.GetLocation(),
                                position.Value,
                                typeInfo.Type);
                            syntaxNodeContext.ReportDiagnostic(diagnostic);
                            continue;
                        }
                    }

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
                            continue;
                        }

                        var typeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(assignmentExpressionSyntax);
                        if (typeInfo.Type != null && !Equals(typeInfo.Type, substituteCallParameters[position.Value].Type))
                        {
                            var diagnostic = Diagnostic.Create(
                                DiagnosticDescriptorsProvider.CallInfoArgumentSetWithIncompatibleValue,
                                indexer.GetLocation(),
                                typeInfo.Type,
                                position.Value,
                                substituteCallParameters[position.Value].Type);
                            syntaxNodeContext.ReportDiagnostic(diagnostic);
                            continue;
                        }
                    }
                }
            }
        }

        private bool SupportsCallInfo(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax syntax, IMethodSymbol methodSymbol)
        {
            var allArguments = GetArgumentExpressions(syntax);
            var argumentsForAnalysis = methodSymbol.MethodKind == MethodKind.ReducedExtension
                ? allArguments
                : allArguments.Skip(1);

            if (MethodNames.TryGetValue(methodSymbol.Name, out var typeName) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            var supportsCallInfo =
                symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                symbol.Symbol?.ContainingType?.ToString().Equals(typeName, StringComparison.OrdinalIgnoreCase) == true;

            return supportsCallInfo && IsCalledViaDelegate(syntaxNodeContext.SemanticModel, syntaxNodeContext.SemanticModel.GetTypeInfo(argumentsForAnalysis.First()));
        }

        private IList<IParameterSymbol> GetSubstituteCallParameters(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, TInvocationExpressionSyntax invocationExpression)
        {
            var parentMethodCallSyntax = GetSubstituteCall(methodSymbol, invocationExpression);
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

        private static bool IsCalledViaDelegate(SemanticModel semanticModel, TypeInfo typeInfo)
        {
            var typeSymbol = typeInfo.Type ?? typeInfo.ConvertedType;
            var isCalledViaDelegate = typeSymbol != null &&
                                      typeSymbol.TypeKind == TypeKind.Delegate &&
                                      typeSymbol is INamedTypeSymbol namedTypeSymbol &&
                                      namedTypeSymbol.ConstructedFrom.Equals(
                                          semanticModel.Compilation.GetTypeByMetadataName("System.Func`2")) &&
                                      IsCallInfoParameter(namedTypeSymbol.TypeArguments.First());

            return isCalledViaDelegate;
        }

        private static bool IsCallInfoParameter(ITypeSymbol symbol)
        {
            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ToString().Equals(MetadataNames.NSubstituteCoreFullTypeName, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}