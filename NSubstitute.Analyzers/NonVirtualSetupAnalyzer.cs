using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonVirtualSetupAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptors.NonVirtualSetupSpecification);

        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReturnsMethod,
            MetadataNames.NSubstituteReturnsForAnyArgsMethod);

        private static readonly ImmutableHashSet<int> SupportedMemberAccesses = ImmutableHashSet.Create(
            (int) SyntaxKind.InvocationExpression,
            (int) SyntaxKind.SimpleMemberAccessExpression,
            (int) SyntaxKind.ElementAccessExpression,
            (int) SyntaxKind.NumericLiteralExpression,
            (int) SyntaxKind.CharacterLiteralExpression,
            (int) SyntaxKind.FalseLiteralExpression,
            (int) SyntaxKind.TrueLiteralExpression,
            (int) SyntaxKind.StringLiteralExpression);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
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

            if (MethodNames.Contains(methodSymbol.Name) == false)
            {
                return;
            }

            var type = syntaxNodeContext.Compilation.GetTypeByMetadataName(MetadataNames.NSubstituteSubstituteExtensions);

            if (type == null)
            {
                return;
            }

            if (methodSymbol.ContainingType != type)
            {
                return;
            }

            var argumentSyntax = invocationExpression.ArgumentList.Arguments.FirstOrDefault()?.DescendantNodes().FirstOrDefault();

            AnalyzeMember(syntaxNodeContext, argumentSyntax);
        }

        private void AnalyzeMemberAccess(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax) syntaxNodeContext.Node;
            if (MethodNames.Contains(memberAccessExpression.Name.Identifier.Text) == false)
            {
                return;
            }

            var type = syntaxNodeContext.Compilation.GetTypeByMetadataName(
                MetadataNames.NSubstituteSubstituteExtensions);

            if (type == null)
            {
                return;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(memberAccessExpression);

            if (symbol.Symbol?.ContainingType != type)
            {
                return;
            }

            var accessedMember = memberAccessExpression.DescendantNodes().FirstOrDefault();

            AnalyzeMember(syntaxNodeContext, accessedMember);
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
            return accessedMember != null && SupportedMemberAccesses.Contains(accessedMember.RawKind);
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