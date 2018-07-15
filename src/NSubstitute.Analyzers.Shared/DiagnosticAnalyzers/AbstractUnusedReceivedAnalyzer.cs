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
        protected AbstractUnusedReceivedAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptorsProvider.UnusedReceived);

        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReceivedMethod,
            MetadataNames.NSubstituteReceivedWithAnyArgsMethod,
            MetadataNames.NSubstituteDidNotReceiveMethod,
            MetadataNames.NSubstituteDidNotReceiveWithAnyArgsMethod);

        protected abstract ImmutableArray<Parent> PossibleParents { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        public sealed override void Initialize(AnalysisContext context)
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

        private bool IsConsideredAsUsed(SyntaxNode receivedSyntaxNode)
        {
            var typeInfo = receivedSyntaxNode.Parent.GetType().GetTypeInfo();

            return PossibleParents.Any(parent => parent.Type.GetTypeInfo().IsAssignableFrom(typeInfo));
        }
    }

    internal struct Parent
    {
        public Type Type { get; }

        private Parent(Type type)
        {
            Type = type;
        }

        public static Parent Create<T>()
        {
            return new Parent(typeof(T));
        }
    }
}