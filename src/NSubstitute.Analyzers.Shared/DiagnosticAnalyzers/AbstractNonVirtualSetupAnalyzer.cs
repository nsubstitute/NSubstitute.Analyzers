using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;
using NSubstitute.Analyzers.Shared.Settings;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonVirtualSetupAnalyzer<TSyntaxKind, TMemberAccessExpressionSyntax, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TMemberAccessExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReturnsMethod,
            MetadataNames.NSubstituteReturnsForAnyArgsMethod);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptorsProvider.NonVirtualSetupSpecification);

        protected abstract ImmutableHashSet<int> SupportedMemberAccesses { get; }

        protected abstract ImmutableHashSet<Type> KnownNonVirtualSyntaxTypes { get; }

        protected abstract TSyntaxKind SimpleMemberAccessExpressionKind { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractNonVirtualSetupAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
        }

        public sealed override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SimpleMemberAccessExpressionKind);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, InvocationExpressionKind);
        }

        protected abstract SyntaxNode GetArgument(TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract string GetAccessedMemberName(TMemberAccessExpressionSyntax memberAccessExpressionSyntax);

        protected virtual bool? CanBeSetuped(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode accessedMember, SymbolInfo symbolInfo)
        {
            if (KnownNonVirtualSyntaxTypes.Contains(accessedMember.GetType()))
            {
                return false;
            }

            if (symbolInfo.Symbol == null)
            {
                return null;
            }

            return IsInterfaceMember(symbolInfo) || IsVirtual(symbolInfo) || IsSupressed(syntaxNodeContext, symbolInfo.Symbol);
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
            if (methodSymbol == null || methodSymbol.MethodKind != MethodKind.Ordinary)
            {
                return;
            }

            if (IsReturnsLikeMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
            {
                return;
            }

            var argumentSyntax = GetArgument((TInvocationExpressionSyntax)invocationExpression);

            AnalyzeMember(syntaxNodeContext, argumentSyntax);
        }

        private void AnalyzeMemberAccess(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var memberAccessExpression = syntaxNodeContext.Node;
            var memberName = GetAccessedMemberName((TMemberAccessExpressionSyntax)memberAccessExpression);
            if (IsReturnsLikeMethod(syntaxNodeContext, memberAccessExpression, memberName) == false)
            {
                return;
            }

            var accessedMember = memberAccessExpression.DescendantNodes().FirstOrDefault();

            AnalyzeMember(syntaxNodeContext, accessedMember);
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

        private void AnalyzeMember(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode accessedMember)
        {
            if (IsValidForAnalysis(accessedMember) == false)
            {
                return;
            }

            var accessedSymbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(accessedMember);

            var canBeSetuped = CanBeSetuped(syntaxNodeContext, accessedMember, accessedSymbol);
            if (canBeSetuped.HasValue && canBeSetuped == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.NonVirtualSetupSpecification,
                    accessedMember.GetLocation(),
                    accessedSymbol.Symbol?.Name ?? accessedMember.ToString());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }

        private bool IsValidForAnalysis(SyntaxNode accessedMember)
        {
            return accessedMember != null && SupportedMemberAccesses.Contains(accessedMember.RawKind);
        }

        private bool IsInterfaceMember(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.ContainingType?.TypeKind == TypeKind.Interface;
        }

        private bool IsVirtual(SymbolInfo symbolInfo)
        {
            var member = symbolInfo.Symbol;

            var isVirtual = member.IsVirtual
                            || (member.IsOverride && !member.IsSealed)
                            || member.IsAbstract;

            return isVirtual;
        }

        private bool IsSupressed(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol)
        {
            if (symbol == null)
            {
                return false;
            }

            var analyzersSettings = syntaxNodeContext.GetSettings(CancellationToken.None);

            return IsSupressed(syntaxNodeContext, analyzersSettings, symbol);
        }

        private bool IsSupressed(SyntaxNodeAnalysisContext syntaxNodeContext, AnalyzersSettings settings, ISymbol symbol)
        {
            foreach (var supression in settings.Suppressions.Where(suppression => suppression.Rules.Contains(DiagnosticDescriptorsProvider.NonVirtualSetupSpecification.Id)))
            {
                foreach (var supressedSymbol in DocumentationCommentId.GetSymbolsForDeclarationId(supression.Target, syntaxNodeContext.Compilation))
                {
                    if (supressedSymbol.Equals(symbol) ||
                        supressedSymbol.Equals(symbol.ContainingType) ||
                        supressedSymbol.Equals(symbol.ContainingNamespace) ||
                        (symbol is IMethodSymbol methodSymbol && methodSymbol.ConstructedFrom.Equals(supressedSymbol)) ||
                        (symbol.ContainingType is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.ConstructedFrom.Equals(supressedSymbol)) ||
                        (symbol is IPropertySymbol propertySymbol && propertySymbol.OriginalDefinition.Equals(supressedSymbol)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}