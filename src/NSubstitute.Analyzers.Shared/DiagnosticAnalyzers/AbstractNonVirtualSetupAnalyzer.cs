using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonVirtualSetupAnalyzer<TSyntaxKind, TMemberAccessExpressionSyntax, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TMemberAccessExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        private static readonly ImmutableDictionary<string, string> MethodNamesMap = new Dictionary<string, string>
            {
                [MetadataNames.NSubstituteReturnsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
                [MetadataNames.NSubstituteReturnsForAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
                [MetadataNames.NSubstituteThrowsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName,
                [MetadataNames.NSubstituteThrowsForAnyArgsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName
            }.ToImmutableDictionary();

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

            return symbolInfo.Symbol?.CanBeSetuped();
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

            if (IsSetupLikeMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
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
            if (IsSetupLikeMethod(syntaxNodeContext, memberAccessExpression, memberName) == false)
            {
                return;
            }

            var accessedMember = memberAccessExpression.DescendantNodes().FirstOrDefault();

            AnalyzeMember(syntaxNodeContext, accessedMember);
        }

        private bool IsSetupLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax, string memberName)
        {
            if (MethodNamesMap.TryGetValue(memberName, out var containingType) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            return symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.Symbol?.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) == true;
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

                TryReportDiagnostic(syntaxNodeContext, diagnostic, accessedSymbol.Symbol);
            }
        }

        private bool IsValidForAnalysis(SyntaxNode accessedMember)
        {
            return accessedMember != null && SupportedMemberAccesses.Contains(accessedMember.RawKind);
        }
    }
}