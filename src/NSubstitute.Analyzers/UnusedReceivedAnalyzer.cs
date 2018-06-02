using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
#if CSHARP
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
#endif
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
#if VISUAL_BASIC
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

namespace NSubstitute.Analyzers
{
#if CSHARP
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
#elif VISUAL_BASIC
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
#endif
    public class UnusedReceivedAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptors.UnusedReceived);

        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReceivedMethod,
            MetadataNames.NSubstituteReceivedWithAnyArgsMethod,
            MetadataNames.NSubstituteDidNotReceiveMethod,
            MetadataNames.NSubstituteDidNotReceiveWithAnyArgsMethod);

#if CSHARP
        private static readonly ImmutableArray<Parent> PossibleParents =
            ImmutableArray.Create(
                Parent.Create<MemberAccessExpressionSyntax>(),
                Parent.Create<InvocationExpressionSyntax>(),
                Parent.Create<ElementAccessExpressionSyntax>());
#elif VISUAL_BASIC
        private static readonly ImmutableArray<Parent> PossibleParents =
            ImmutableArray.Create(
                Parent.Create<MemberAccessExpressionSyntax>(),
                Parent.Create<InvocationExpressionSyntax>());
#endif

        public sealed override void Initialize(AnalysisContext context)
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

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.UnusedReceived,
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

        private struct Parent
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
}