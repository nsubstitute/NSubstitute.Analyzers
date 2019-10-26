using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractArgumentMatcherAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        protected abstract ImmutableArray<ImmutableArray<int>> AllowedAncestorPaths { get; }

        protected abstract ImmutableArray<ImmutableArray<int>> IgnoredAncestorPaths { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractArgumentMatcherAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            _analyzeInvocationAction = AnalyzeInvocation;
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

            if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            var symbol = methodSymbolInfo.Symbol;

            if (symbol.IsArgMatcherLikeMethod() == false)
            {
                return;
            }

            AnalyzeArgLikeMethod(syntaxNodeContext, invocationExpression);
        }

        private void AnalyzeArgLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax argInvocationExpression)
        {
            // find allowed enclosing expression
            var enclosingExpression = FindAllowedEnclosingExpression(argInvocationExpression);

            // if Arg is used with not allowed expression, find if it is used in ignored ones eg. var x = Arg.Any
            // as variable might be used later on
            if (enclosingExpression == null && FindIgnoredEnclosingExpression(argInvocationExpression) == null)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.ArgumentMatcherUsedWithoutSpecifyingCall,
                    argInvocationExpression.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return;
            }

            if (enclosingExpression == null)
            {
                return;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(enclosingExpression).Symbol;
            var canBeSetuped = symbol.CanBeSetuped();

            if (canBeSetuped == false || symbol.MemberVisibleToProxyGenerator() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.ArgumentMatcherUsedWithoutSpecifyingCall,
                    argInvocationExpression.GetLocation());

                TryReportDiagnostic(syntaxNodeContext, diagnostic, symbol);
            }
        }

        private SyntaxNode FindAllowedEnclosingExpression(TInvocationExpressionSyntax invocationExpression)
        {
            return invocationExpression.GetAncestorNode(AllowedAncestorPaths);
        }

        private SyntaxNode FindIgnoredEnclosingExpression(TInvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.GetAncestorNode(IgnoredAncestorPaths);
        }
    }
}