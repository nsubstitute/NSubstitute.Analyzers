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

        private static readonly ImmutableArray<ImmutableArray<Parent>> PossibleHierarchies = ImmutableArray.Create(
            ImmutableArray.Create(Parent.Create<MemberAccessExpressionSyntax>())
#if CSHARP
            ,ImmutableArray.Create(Parent.Create<ElementAccessExpressionSyntax>())
#endif
    );

        public sealed override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (InvocationExpressionSyntax) syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

            if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            var methodSymbol = (IMethodSymbol) methodSymbolInfo.Symbol;
            if (methodSymbol == null || methodSymbol.MethodKind != MethodKind.ReducedExtension)
            {
                return;
            }

            if (IsReceivedLikeMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
            {
                return;
            }

            var isConsideredAsUsed = IsConsideredAsUsed(invocationExpression);

            if (isConsideredAsUsed == true)
            {
                return;
            }

            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.UnusedReceived,
                invocationExpression.GetLocation(),
                methodSymbol.Name);

            syntaxNodeContext.ReportDiagnostic(diagnostic);
        }

        private void AnalyzeMemberAccess(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax) syntaxNodeContext.Node;
            var memberName = memberAccessExpression.Name.Identifier.Text;
        }

        private static bool IsReceivedLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax,
            string memberName)
        {
            if (MethodNames.Contains(memberName) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            return symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName,
                       StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.Symbol?.ContainingType?.ToString().Equals(
                       MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
                       StringComparison.OrdinalIgnoreCase) == true;
        }

        private bool IsConsideredAsUsed(SyntaxNode receivedSyntaxNode)
        {
            return PossibleHierarchies.Any(hierarchy => FindInvocationInHierarchy(receivedSyntaxNode, hierarchy) != null);
        }

        private static SyntaxNode FindInvocationInHierarchy(SyntaxNode node, IList<Parent> ancestors)
        {
            SyntaxNode parent = null;
            foreach (var expectedAncestor in ancestors)
            {
                parent = node.Parent;
                if (expectedAncestor.Type.GetTypeInfo().IsAssignableFrom(parent.GetType().GetTypeInfo()) == false)
                {
                    return null;
                }

                node = parent;
            }

            return parent;
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