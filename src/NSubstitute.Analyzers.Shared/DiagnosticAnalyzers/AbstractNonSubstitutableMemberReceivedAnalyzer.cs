using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonSubstitutableMemberReceivedAnalyzer<TSyntaxKind, TMemberAccessExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
        where TMemberAccessExpressionSyntax : SyntaxNode
    {
        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;
        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReceivedMethod,
            MetadataNames.NSubstituteReceivedWithAnyArgsMethod,
            MetadataNames.NSubstituteDidNotReceiveMethod,
            MetadataNames.NSubstituteDidNotReceiveWithAnyArgsMethod);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected AbstractNonSubstitutableMemberReceivedAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            _analyzeInvocationAction = AnalyzeInvocation;
            SupportedDiagnostics = ImmutableArray.Create(
                DiagnosticDescriptorsProvider.NonVirtualReceivedSetupSpecification,
                DiagnosticDescriptorsProvider.InternalSetupSpecification);
        }

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

            var parentNode = GetKnownParent(invocationExpression);

            if (parentNode == null)
            {
                return;
            }

            var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(parentNode);

            if (symbolInfo.Symbol == null)
            {
                return;
            }

            var canBeSetuped = symbolInfo.Symbol.CanBeSetuped();

            if (canBeSetuped == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.NonVirtualReceivedSetupSpecification,
                    GetSubstitutionNodeActualLocation(parentNode, symbolInfo.Symbol),
                    symbolInfo.Symbol.Name);

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }

            if (canBeSetuped && symbolInfo.Symbol != null && symbolInfo.Symbol.MemberVisibleToProxyGenerator() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.InternalSetupSpecification,
                    GetSubstitutionNodeActualLocation(parentNode, symbolInfo.Symbol),
                    symbolInfo.Symbol.Name);

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsReceivedLikeMethod(ISymbol symbol)
        {
            if (MethodNames.Contains(symbol.Name) == false)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.Ordinal) == true &&
                   symbol.ContainingType?.ToString().Equals(MetadataNames.NSubstituteSubstituteExtensionsFullTypeName, StringComparison.Ordinal) == true;
        }

        private SyntaxNode GetKnownParent(SyntaxNode receivedSyntaxNode)
        {
            return PossibleParentsRawKinds.Contains(receivedSyntaxNode.Parent.RawKind) ? receivedSyntaxNode.Parent : null;
        }

        private Location GetSubstitutionNodeActualLocation(SyntaxNode syntaxNode, ISymbol symbol)
        {
            return syntaxNode.GetSubstitutionNodeActualLocation<TMemberAccessExpressionSyntax>(symbol);
        }
    }
}