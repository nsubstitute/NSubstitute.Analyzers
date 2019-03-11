using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptorsProvider.ConflictingAssignmentsToOutRefArgument);

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

        protected abstract ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

        protected abstract AbstractCallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax> GetCallInfoFinder();

        protected abstract int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

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

            var expressionSyntax = GetArgumentExpressions(invocationExpression).First();

            // analyze assignments
            var andDoesIndexers = GetCallInfoFinder().GetCallInfoContext(syntaxNodeContext.SemanticModel, expressionSyntax);
            var previousCallIndexer = GetArgumentExpressions(previousCall).Select(arg => GetCallInfoFinder().GetCallInfoContext(syntaxNodeContext.SemanticModel, arg));

            var immutableHashSet = previousCallIndexer.SelectMany(x => x.IndexerAccesses).Where(acc => GetIndexerInfo(syntaxNodeContext, acc).VerifyAssignment && GetAssignmentExpression(acc) != null)
                .Select(x => GetIndexerPosition(syntaxNodeContext, x)).ToImmutableHashSet();

            foreach (var indexerExpressionSyntax in andDoesIndexers.IndexerAccesses)
            {
                // TODO
                var info = GetIndexerInfo(syntaxNodeContext, indexerExpressionSyntax);
                if (info.VerifyAssignment && GetAssignmentExpression(indexerExpressionSyntax) != null)
                {
                    var position = GetIndexerPosition(syntaxNodeContext, indexerExpressionSyntax);
                    if (position.HasValue && immutableHashSet.Contains(position.Value))
                    {
                        syntaxNodeContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptorsProvider.ConflictingAssignmentsToOutRefArgument, indexerExpressionSyntax.GetLocation()));
                    }
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

        private IndexerInfo GetIndexerInfo(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax)
        {
            var info = GetIndexerSymbol(syntaxNodeAnalysisContext, indexerExpressionSyntax);
            var symbol = info as IMethodSymbol;
            var verifyIndexerCast = symbol == null || symbol.Name != MetadataNames.CallInfoArgTypesMethod;
            var verifyAssignment = symbol == null;

            var indexerInfo = new IndexerInfo(verifyIndexerCast, verifyAssignment);
            return indexerInfo;
        }

        private struct IndexerInfo
        {
            public bool VerifyIndexerCast { get; }

            public bool VerifyAssignment { get; }

            public IndexerInfo(bool verifyIndexerCast, bool verifyAssignment)
            {
                VerifyIndexerCast = verifyIndexerCast;
                VerifyAssignment = verifyAssignment;
            }
        }
    }
}