using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractReEntrantSetupAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        private readonly IReEntrantCallFinder _reEntrantCallFinder;
        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReturnsMethod,
            MetadataNames.NSubstituteReturnsForAnyArgsMethod);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected AbstractReEntrantSetupAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider, IReEntrantCallFinder reEntrantCallFinder)
            : base(diagnosticDescriptorsProvider)
        {
            _reEntrantCallFinder = reEntrantCallFinder;
            _analyzeInvocationAction = AnalyzeInvocation;
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.ReEntrantSubstituteCall);
        }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
        }

        protected abstract IEnumerable<SyntaxNode> ExtractArguments(TInvocationExpressionSyntax invocationExpressionSyntax);

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

            if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;

            if (IsReturnsLikeMethod(methodSymbol) == false)
            {
                return;
            }

            var allArguments = ExtractArguments(invocationExpression);
            var argumentsForAnalysis = methodSymbol.MethodKind == MethodKind.ReducedExtension ? allArguments : allArguments.Skip(1);

            foreach (var argument in argumentsForAnalysis)
            {
                var reentrantSymbol = _reEntrantCallFinder.GetReEntrantCalls(syntaxNodeContext.Compilation, syntaxNodeContext.SemanticModel, invocationExpression, argument).FirstOrDefault();
                if (reentrantSymbol != null)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.ReEntrantSubstituteCall,
                        argument.GetLocation(),
                        methodSymbol.Name,
                        reentrantSymbol.Name,
                        argument.ToString());

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                }
            }
        }

        private bool IsReturnsLikeMethod(ISymbol symbol)
        {
            if (MethodNames.Contains(symbol.Name) == false)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ContainingType?.ToString().Equals(MetadataNames.NSubstituteSubstituteExtensionsFullTypeName, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}