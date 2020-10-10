using System.Collections.Generic;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal class CallInfoContext<TInvocationExpressionSyntax, TIndexerSyntax>
    {
        public IReadOnlyList<TIndexerSyntax> IndexerAccesses { get; }

        public IReadOnlyList<TInvocationExpressionSyntax> ArgAtInvocations { get; }

        public IReadOnlyList<TInvocationExpressionSyntax> ArgInvocations { get; }

        public CallInfoContext(
            IReadOnlyList<TInvocationExpressionSyntax> argAtInvocations,
            IReadOnlyList<TInvocationExpressionSyntax> argInvocations,
            IReadOnlyList<TIndexerSyntax> indexerAccesses)
        {
            IndexerAccesses = indexerAccesses;
            ArgAtInvocations = argAtInvocations;
            ArgInvocations = argInvocations;
        }
    }
}