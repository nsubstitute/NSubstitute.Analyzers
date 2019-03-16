using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractConflictingArgumentAssignmentsAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TExpressionSyntax, TIndexerExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TExpressionSyntax : SyntaxNode
        where TIndexerExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        protected AbstractConflictingArgumentAssignmentsAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.ConflictingArgumentAssignments);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        private static readonly ImmutableDictionary<string, string> MethodNames = new Dictionary<string, string>()
        {
            [MetadataNames.NSubstituteAndDoesMethod] = MetadataNames.NSubstituteConfiguredCallFullTypeName
        }.ToImmutableDictionary();

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, InvocationExpressionKind);
        }

        protected abstract IEnumerable<TExpressionSyntax> GetArgumentExpressions(TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract SyntaxNode GetSubstituteCall(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract AbstractCallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax> GetCallInfoFinder();

        protected abstract int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

        protected abstract ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

        protected abstract SyntaxNode GetAssignmentExpression(TIndexerExpressionSyntax indexerExpressionSyntax);

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

            if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;

            if (IsAndDoesLikeMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
            {
                return;
            }

            var previousCall = GetSubstituteCall(syntaxNodeContext, methodSymbol, invocationExpression) as TInvocationExpressionSyntax;

            if (previousCall == null)
            {
                return;
            }

            var andDoesIndexers = FindCallInfoIndexers(syntaxNodeContext, invocationExpression);
            var previousCallIndexers = FindCallInfoIndexers(syntaxNodeContext, previousCall);

            var immutableHashSet = previousCallIndexers.Select(indexerExpression => GetIndexerPosition(syntaxNodeContext, indexerExpression))
                .ToImmutableHashSet();

            foreach (var indexerExpressionSyntax in andDoesIndexers)
            {
                var position = GetIndexerPosition(syntaxNodeContext, indexerExpressionSyntax);
                if (position.HasValue && immutableHashSet.Contains(position.Value))
                {
                    syntaxNodeContext.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptorsProvider.ConflictingArgumentAssignments,
                        indexerExpressionSyntax.GetLocation()));
                }
            }
        }

        private bool IsAndDoesLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax, string memberName)
        {
            if (MethodNames.TryGetValue(memberName, out var containingType) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            return symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.Symbol?.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) == true;
        }

        private bool IsAssigned(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax)
        {
            return GetAssignmentExpression(indexerExpressionSyntax) != null &&
                   GetIndexerSymbol(syntaxNodeAnalysisContext, indexerExpressionSyntax) is IPropertySymbol propertySymbol &&
                   propertySymbol.ContainingType.IsCallInfoSymbol();
        }

        private IEnumerable<TIndexerExpressionSyntax> FindCallInfoIndexers(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpressionSyntax)
        {
            return GetArgumentExpressions(invocationExpressionSyntax).SelectMany(argument => GetCallInfoFinder().GetCallInfoContext(syntaxNodeContext.SemanticModel, argument).IndexerAccesses)
                .Where(indexerExpression => IsAssigned(syntaxNodeContext, indexerExpression));
        }
    }
}