using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonSubstitutableMemberArgumentMatcherAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        private readonly INonSubstitutableMemberAnalysis _nonSubstitutableMemberAnalysis;

        protected abstract ImmutableArray<ImmutableArray<int>> AllowedAncestorPaths { get; }

        protected abstract ImmutableArray<ImmutableArray<int>> IgnoredAncestorPaths { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractNonSubstitutableMemberArgumentMatcherAnalyzer(
            INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis,
            IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            _nonSubstitutableMemberAnalysis = nonSubstitutableMemberAnalysis;
            _analyzeInvocationAction = AnalyzeInvocation;
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage);
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
                    DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage,
                    argInvocationExpression.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return;
            }

            if (enclosingExpression == null)
            {
                return;
            }

            var enclosingExpressionSymbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(enclosingExpression).Symbol;

            if (enclosingExpressionSymbol == null)
            {
                return;
            }

            var analysisResult = _nonSubstitutableMemberAnalysis.Analyze(syntaxNodeContext, enclosingExpression);

            if (analysisResult.CanBeSubstituted == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage,
                    argInvocationExpression.GetLocation());

                syntaxNodeContext.TryReportDiagnostic(diagnostic, enclosingExpressionSymbol);
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