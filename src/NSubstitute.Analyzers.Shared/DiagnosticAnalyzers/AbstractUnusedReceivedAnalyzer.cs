using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractUnusedReceivedAnalyzer<TSyntaxKind> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
    {
        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        protected AbstractUnusedReceivedAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            _analyzeInvocationAction = AnalyzeInvocation;
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.UnusedReceived);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReceivedMethod,
            MetadataNames.NSubstituteReceivedWithAnyArgsMethod,
            MetadataNames.NSubstituteDidNotReceiveMethod,
            MetadataNames.NSubstituteDidNotReceiveWithAnyArgsMethod);

        protected abstract ImmutableHashSet<int> PossibleParentsRawKinds { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

            if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;

            if (IsReceivedLikeMethod(methodSymbol) == false)
            {
                return;
            }

            var isConsideredAsUsed = IsConsideredAsUsed(invocationExpression);

            if (isConsideredAsUsed)
            {
                return;
            }

            var diagnosticDescriptor = methodSymbol.MethodKind == MethodKind.Ordinary
                ? DiagnosticDescriptorsProvider.UnusedReceivedForOrdinaryMethod
                : DiagnosticDescriptorsProvider.UnusedReceived;

            var diagnostic = Diagnostic.Create(
                diagnosticDescriptor,
                invocationExpression.GetLocation(),
                methodSymbol.Name);

            syntaxNodeContext.ReportDiagnostic(diagnostic);
        }

        private static bool IsReceivedLikeMethod(IMethodSymbol symbol)
        {
            if (MethodNames.Contains(symbol.Name) == false)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.Ordinal) == true &&
                   symbol.ContainingType?.ToString().Equals(MetadataNames.NSubstituteSubstituteExtensionsFullTypeName, StringComparison.Ordinal) == true;
        }

        private bool IsConsideredAsUsed(SyntaxNode receivedSyntaxNode)
        {
            return PossibleParentsRawKinds.Contains(receivedSyntaxNode.Parent.RawKind);
        }
    }
}