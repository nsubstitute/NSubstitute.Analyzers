using System;
using System.Collections.Immutable;
using System.Linq;
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
    public class NonVirtualSetupAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptors.NonVirtualSetupSpecification);

        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReturnsMethod,
            MetadataNames.NSubstituteReturnsForAnyArgsMethod);

        private static readonly ImmutableHashSet<SyntaxKind> SupportedMemberAccesses = ImmutableHashSet.Create(
            SyntaxKind.InvocationExpression,
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxKind.ElementAccessExpression,
            SyntaxKind.NumericLiteralExpression,
            SyntaxKind.CharacterLiteralExpression,
            SyntaxKind.FalseLiteralExpression,
            SyntaxKind.TrueLiteralExpression,
            SyntaxKind.StringLiteralExpression);

        public sealed override void Initialize(AnalysisContext context)
        {context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (InvocationExpressionSyntax) syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);
            var methodSymbol = (IMethodSymbol) methodSymbolInfo.Symbol;
            if (methodSymbol == null || methodSymbol.MethodKind != MethodKind.Ordinary)
            {
                return;
            }

            if (IsSubstituteMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
            {
                return;
            }

            var argumentSyntax = invocationExpression.ArgumentList.Arguments.FirstOrDefault()?.DescendantNodes().FirstOrDefault();

            AnalyzeMember(syntaxNodeContext, argumentSyntax);
        }

        private void AnalyzeMemberAccess(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax) syntaxNodeContext.Node;
            var memberName = memberAccessExpression.Name.Identifier.Text;
            if (IsSubstituteMethod(syntaxNodeContext, memberAccessExpression, memberName) == false)
            {
                return;
            }

            var accessedMember = memberAccessExpression.DescendantNodes().FirstOrDefault();

            AnalyzeMember(syntaxNodeContext, accessedMember);
        }

        private static bool IsSubstituteMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax, string memberName)
        {
            if (MethodNames.Contains(memberName) == false)
            {
                return false;
            }

            var type = syntaxNodeContext.Compilation.GetTypeByMetadataName(
                MetadataNames.NSubstituteSubstituteExtensions);

            if (type == null)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            if (symbol.Symbol?.ContainingType != type && symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == false)
            {
                return false;
            }

            return true;
        }

        private static void AnalyzeMember(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode accessedMember)
        {
            if (IsValidForAnalysis(accessedMember) == false)
            {
                return;
            }

            var accessedSymbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(accessedMember);

            if (accessedMember is LiteralExpressionSyntax literalExpressionSyntax)
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.NonVirtualSetupSpecification,
                    accessedMember.GetLocation(),
                    literalExpressionSyntax.ToString());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }

            if (accessedSymbol.Symbol != null && CanBeSetuped(accessedSymbol) == false)
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.NonVirtualSetupSpecification,
                    accessedMember.GetLocation(),
                    accessedSymbol.Symbol.Name);

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsValidForAnalysis(SyntaxNode accessedMember)
        {
            return accessedMember != null && SupportedMemberAccesses.Contains(accessedMember.Kind());
        }

        private static bool CanBeSetuped(SymbolInfo symbolInfo)
        {
            return IsInterfaceMember(symbolInfo) || IsVirtual(symbolInfo);
        }

        private static bool IsInterfaceMember(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.ContainingType?.TypeKind == TypeKind.Interface;
        }

        private static bool IsVirtual(SymbolInfo symbolInfo)
        {
            var member = symbolInfo.Symbol;

            var isVirtual = member.IsVirtual
                            || (member.IsOverride && !member.IsSealed)
                            || member.IsAbstract;

            return isVirtual;
        }
    }
}