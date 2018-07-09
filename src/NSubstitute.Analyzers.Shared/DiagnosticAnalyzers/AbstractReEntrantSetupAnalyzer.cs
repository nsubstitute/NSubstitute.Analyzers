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
        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReturnsMethod,
            MetadataNames.NSubstituteReturnsForAnyArgsMethod);

        private AbstractReEntrantCallFinder ReEntrantCallFinder => _reEntrantCallFinderProxy.Value;

        private readonly Lazy<AbstractReEntrantCallFinder> _reEntrantCallFinderProxy;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptorsProvider.ReEntrantSubstituteCall);

        protected AbstractReEntrantSetupAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            _reEntrantCallFinderProxy = new Lazy<AbstractReEntrantCallFinder>(GetReEntrantCallFinder);
        }

        protected abstract AbstractReEntrantCallFinder GetReEntrantCallFinder();

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, InvocationExpressionKind);
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

            if (IsReturnsLikeMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
            {
                return;
            }

            var allArguments = ExtractArguments(invocationExpression);
            var argumentsForAnalysis = methodSymbol.MethodKind == MethodKind.ReducedExtension ? allArguments : allArguments.Skip(1);

            foreach (var argument in argumentsForAnalysis)
            {
                var reentrantSymbol = ReEntrantCallFinder.GetReEntrantCalls(syntaxNodeContext.SemanticModel, argument).FirstOrDefault();
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

        private bool IsReturnsLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax, string memberName)
        {
            if (MethodNames.Contains(memberName) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            return symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.Symbol?.ContainingType?.ToString().Equals(MetadataNames.NSubstituteSubstituteExtensionsFullTypeName, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}