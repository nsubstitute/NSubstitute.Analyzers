using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class CallInfoAnalyzer : AbstractDiagnosticAnalyzer
    {
        public CallInfoAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReturnsMethod,
            MetadataNames.NSubstituteReturnsForAnyArgsMethod);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
            DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
            DiagnosticDescriptorsProvider.CallInfoCouldNotFindArgumentToThisCall,
            DiagnosticDescriptorsProvider.CallInfoMoreThanOneArgumentOfType,
            DiagnosticDescriptorsProvider.CallInfoArgumentSetWithIncompatibleValue,
            DiagnosticDescriptorsProvider.CallInfoArgumentIsNotOutOrRef);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (InvocationExpressionSyntax)syntaxNodeContext.Node;
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

            var memberAccessExpression = invocationExpression.Expression.DescendantNodes().First();

            var parentCallInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
            if (parentCallInfo == null)
            {
                return;
            }

            foreach (var argument in invocationExpression.ArgumentList.Arguments)
            {
                var finder = new CallInfoCallFinder();
                var callInfoContext = finder.GetCallInfoContext(syntaxNodeContext.SemanticModel, argument.Expression);
                foreach (var argAtInvocation in callInfoContext.ArgAtInvocations)
                {
                    var position = syntaxNodeContext.SemanticModel.GetConstantValue(argAtInvocation.ArgumentList.Arguments.First().Expression);
                    if (position.HasValue && position.Value is int intPosition)
                    {
                        if (intPosition > parentCallInfo.Parameters.Length - 1)
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
                            Equals(parentCallInfo.Parameters[intPosition].Type, argAtMethodSymbol.TypeArguments.First()) == false)
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
                        var parameterCount = parentCallInfo.Parameters.Count(param => Equals(param.Type, typeSymbol));
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
                    var info = syntaxNodeContext.SemanticModel.GetSymbolInfo(indexer.Parent.DescendantNodes().First());
                    var symbol = info.Symbol as IMethodSymbol;
                    var verifyIndexerCast = symbol == null || symbol.Name != MetadataNames.CallInfoArgTypesMethod;
                    var verifyAssignment = symbol == null;

                    var position = syntaxNodeContext.SemanticModel.GetConstantValue(indexer.ArgumentList.Arguments.First().Expression);
                    var positionValue = (int)position.Value;
                    if (position.HasValue && positionValue > parentCallInfo.Parameters.Length - 1)
                    {
                        var diagnostic = Diagnostic.Create(
                            DiagnosticDescriptorsProvider.CallInfoArgumentOutOfRange,
                            indexer.GetLocation(),
                            positionValue);

                        syntaxNodeContext.ReportDiagnostic(diagnostic);
                        continue;
                    }

                    if (verifyIndexerCast && indexer.Parent is CastExpressionSyntax castExpressionSyntax)
                    {
                        var typeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(castExpressionSyntax.Type);
                        if (typeInfo.Type != null && !Equals(typeInfo.Type, parentCallInfo.Parameters[positionValue].Type))
                        {
                            var diagnostic = Diagnostic.Create(
                                DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
                                indexer.GetLocation(),
                                positionValue,
                                typeInfo.Type);
                            syntaxNodeContext.ReportDiagnostic(diagnostic);
                            continue;
                        }
                    }

                    if (verifyIndexerCast && indexer.Parent is BinaryExpressionSyntax binaryExpressionSyntax && binaryExpressionSyntax.OperatorToken.Kind() == SyntaxKind.AsKeyword)
                    {
                        var typeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(binaryExpressionSyntax.Right);
                        if (typeInfo.Type != null && !Equals(typeInfo.Type, parentCallInfo.Parameters[positionValue].Type))
                        {
                            var diagnostic = Diagnostic.Create(
                                DiagnosticDescriptorsProvider.CallInfoCouldNotConvertParameterAtPosition,
                                indexer.GetLocation(),
                                positionValue,
                                typeInfo.Type);
                            syntaxNodeContext.ReportDiagnostic(diagnostic);
                            continue;
                        }
                    }

                    if (verifyAssignment && indexer.Parent is AssignmentExpressionSyntax assignmentExpressionSyntax && position.HasValue && positionValue < parentCallInfo.Parameters.Length)
                    {
                        var parameterSymbol = parentCallInfo.Parameters[positionValue];
                        if (parameterSymbol.RefKind != RefKind.Out && parameterSymbol.RefKind != RefKind.Ref)
                        {
                            var diagnostic = Diagnostic.Create(
                                DiagnosticDescriptorsProvider.CallInfoArgumentIsNotOutOrRef,
                                indexer.GetLocation(),
                                positionValue,
                                parameterSymbol.Type);
                            syntaxNodeContext.ReportDiagnostic(diagnostic);
                            continue;
                        }

                        var typeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(assignmentExpressionSyntax.Right);
                        if (typeInfo.Type != null && !Equals(typeInfo.Type, parentCallInfo.Parameters[positionValue].Type))
                        {
                            var diagnostic = Diagnostic.Create(
                                DiagnosticDescriptorsProvider.CallInfoArgumentSetWithIncompatibleValue,
                                indexer.GetLocation(),
                                typeInfo.Type,
                                positionValue,
                                parentCallInfo.Parameters[positionValue].Type);
                            syntaxNodeContext.ReportDiagnostic(diagnostic);
                            continue;
                        }
                    }
                }
            }
        }

        private bool SupportsCallInfo(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax syntax, IMethodSymbol methodSymbol)
        {
            var allArguments = syntax.ArgumentList.Arguments;
            var argumentsForAnalysis = methodSymbol.MethodKind == MethodKind.ReducedExtension
                ? allArguments
                : allArguments.Skip(1);
            if (MethodNames.Contains(methodSymbol.Name) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            var supportsCallInfo =
                symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                symbol.Symbol?.ContainingType?.ToString().Equals(MetadataNames.NSubstituteSubstituteExtensionsFullTypeName, StringComparison.OrdinalIgnoreCase) == true;

            return supportsCallInfo && IsCalledViaDelegate(syntaxNodeContext.SemanticModel, syntaxNodeContext.SemanticModel.GetTypeInfo(argumentsForAnalysis.First().Expression));
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

    internal class CallInfoCallFinder
    {
        public CallInfoContext GetCallInfoContext(SemanticModel semanticModel, SyntaxNode syntaxNode)
        {
            var visitor = new CallInfoVisitor(semanticModel);
            visitor.Visit(syntaxNode);

            return new CallInfoContext(visitor.ArgAtInvocations, visitor.ArgInvocations, visitor.DirectIndexerAccesses);
        }
    }

    internal class CallInfoVisitor : CSharpSyntaxWalker
    {
        private readonly SemanticModel _semanticModel;

        public List<InvocationExpressionSyntax> ArgAtInvocations { get; }

        public List<InvocationExpressionSyntax> ArgInvocations { get; }

        public List<ElementAccessExpressionSyntax> DirectIndexerAccesses { get; }

        public CallInfoVisitor(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            DirectIndexerAccesses = new List<ElementAccessExpressionSyntax>();
            ArgAtInvocations = new List<InvocationExpressionSyntax>();
            ArgInvocations = new List<InvocationExpressionSyntax>();
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(node);

            if (symbolInfo.Symbol != null &&
                symbolInfo.Symbol.ContainingType.ToString().Equals(MetadataNames.NSubstituteCoreFullTypeName))
            {
                if (symbolInfo.Symbol.Name == MetadataNames.CallInfoArgAtMethod)
                {
                    ArgAtInvocations.Add(node);
                }

                if (symbolInfo.Symbol.Name == MetadataNames.CallInfoArgMethod)
                {
                    ArgInvocations.Add(node);
                }
            }

            base.VisitInvocationExpression(node);
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(node).Symbol ?? _semanticModel.GetSymbolInfo(node.Expression).Symbol;
            if (symbolInfo != null && symbolInfo.ContainingType.ToString().Equals(MetadataNames.NSubstituteCoreFullTypeName))
            {
                DirectIndexerAccesses.Add(node);
            }

            base.VisitElementAccessExpression(node);
        }
    }
}