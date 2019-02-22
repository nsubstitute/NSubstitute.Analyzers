using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonSubstitutableMemberReceivedAnalyzer<TSyntaxKind> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
    {
        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReceivedMethod,
            MetadataNames.NSubstituteReceivedWithAnyArgsMethod,
            MetadataNames.NSubstituteDidNotReceiveMethod,
            MetadataNames.NSubstituteDidNotReceiveWithAnyArgsMethod);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            DiagnosticDescriptorsProvider.NonVirtualReceivedSetupSpecification,
            DiagnosticDescriptorsProvider.InternalSetupSpecification);

        protected AbstractNonSubstitutableMemberReceivedAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
        }

        protected abstract ImmutableArray<Parent> PossibleParents { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, InvocationExpressionKind);
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
            if (methodSymbol == null)
            {
                return;
            }

            if (IsReceivedLikeMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
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
                    GetSymbolLocation(parentNode, symbolInfo.Symbol),
                    symbolInfo.Symbol.Name);

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }

            if (canBeSetuped && symbolInfo.Symbol != null && symbolInfo.Symbol.MemberVisibleToProxyGenerator() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.InternalSetupSpecification,
                    GetSymbolLocation(parentNode, symbolInfo.Symbol),
                    symbolInfo.Symbol.Name);

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsReceivedLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax, string memberName)
        {
            if (MethodNames.Contains(memberName) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            return symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.Ordinal) == true &&
                   symbol.Symbol?.ContainingType?.ToString().Equals(MetadataNames.NSubstituteSubstituteExtensionsFullTypeName, StringComparison.Ordinal) == true;
        }

        private SyntaxNode GetKnownParent(SyntaxNode receivedSyntaxNode)
        {
            var typeInfo = receivedSyntaxNode.Parent.GetType().GetTypeInfo();

            if (PossibleParents.Any(parent => parent.Type.GetTypeInfo().IsAssignableFrom(typeInfo)))
            {
                return receivedSyntaxNode.Parent;
            }

            return null;
        }

        private Location GetSymbolLocation(SyntaxNode syntaxNode, ISymbol symbol)
        {
            var actualNode = symbol is IMethodSymbol _ ? syntaxNode.Parent : syntaxNode;

            return actualNode.GetLocation();
        }
    }
}