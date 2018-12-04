using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractArgumentMatcherAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        private static readonly ImmutableDictionary<string, string> ArgMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.ArgIsMethodName] = MetadataNames.NSubstituteArgFullTypeName,
            [MetadataNames.ArgAnyMethodName] = MetadataNames.NSubstituteArgFullTypeName
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<string, string> MockSetupNamesMap = new Dictionary<string, string>
        {
            [MetadataNames.NSubstituteReturnsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsForAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteThrowsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName,
            [MetadataNames.NSubstituteThrowsForAnyArgsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsNullMethod] = MetadataNames.NSubstituteReturnsExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsNullForAnyArgsMethod] = MetadataNames.NSubstituteReturnsExtensionsFullTypeName
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<string, string> ReceivedMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.NSubstituteReceivedMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReceivedWithAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteDidNotReceiveMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteDidNotReceiveWithAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<string, string> WhenMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.NSubstituteWhenMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteWhenForAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName
        }.ToImmutableDictionary();

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractArgumentMatcherAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
        }

        protected abstract SyntaxNode FindEnclosingExpression(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TInvocationExpressionSyntax invocationExpression);

        protected abstract bool IsFollowedBySetupInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax);

        protected abstract bool IsPrecededByReceivedInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax);

        protected abstract bool IsWithinWhenInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax);

        protected abstract bool IsWithinReceivedInOrderInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptorsProvider.ArgumentMatcherUsedOutsideOfCall);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationContext =>
            {
                /*
                // Initialize state in the start action.
                CompilationAnalyzer analyzer = new CompilationAnalyzer(unsecureMethodAttributeType, secureTypeInterfaceType);

                // Register an intermediate non-end action that accesses and modifies the state.
                compilationContext.RegisterSymbolAction(analyzer.AnalyzeSymbol, SymbolKind.NamedType, SymbolKind.Method);

                // Register an end action to report diagnostics based on the final state.
                compilationContext.RegisterCompilationEndAction(analyzer.CompilationEndAction);
                */

                compilationContext.RegisterSyntaxNodeAction(analysisContext => { }, InvocationExpressionKind);

                compilationContext.RegisterCompilationEndAction(analysisContext => { });
            });

            context.RegisterSyntaxNodeAction(AnalyzeInvocation, InvocationExpressionKind);
        }

        protected bool IsSetupLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol)
        {
            if (symbol == null)
            {
                return false;
            }

            if (MockSetupNamesMap.TryGetValue(symbol.Name, out var containingType) == false)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) == true;
        }

        protected bool IsReceivedLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol)
        {
            if (symbol == null)
            {
                return false;
            }

            if (ReceivedMethodNames.TryGetValue(symbol.Name, out var containingType) == false)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) == true;
        }

        protected bool IsWhenLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol)
        {
            if (symbol == null)
            {
                return false;
            }

            if (WhenMethodNames.TryGetValue(symbol.Name, out var containingType) == false)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) == true;
        }

        protected bool IsReceivedInOrderMethod(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol)
        {
            if (symbol == null)
            {
                return false;
            }

            if (symbol.Name != MetadataNames.NSubstituteInOrderMethod)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ContainingType?.ToString().Equals(MetadataNames.NSubstituteReceivedFullTypeName, StringComparison.OrdinalIgnoreCase) == true;
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
            if (methodSymbol == null || methodSymbol.MethodKind != MethodKind.Ordinary)
            {
                return;
            }

            if (IsArgLikeMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
            {
                return;
            }

            // find enclosing type
            var enclosingExpression = FindEnclosingExpression(syntaxNodeContext, invocationExpression);

            if (enclosingExpression != null &&
                IsFollowedBySetupInvocation(syntaxNodeContext, enclosingExpression) == false &&
                IsPrecededByReceivedInvocation(syntaxNodeContext, enclosingExpression) == false &&
                IsWithinWhenInvocation(syntaxNodeContext, enclosingExpression) == false &&
                IsWithinReceivedInOrderInvocation(syntaxNodeContext, enclosingExpression) == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.ArgumentMatcherUsedOutsideOfCall,
                    invocationExpression.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }

        // TODO unify across all analyzers
        private bool IsArgLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax, string memberName)
        {
            if (ArgMethodNames.TryGetValue(memberName, out var containingType) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            return symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.Symbol?.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}