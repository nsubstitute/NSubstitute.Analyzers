using System.Collections.Generic;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal class CallInfoContext<TInvocationExpressionSyntax, TIndexerSyntax>
    {
        public List<TIndexerSyntax> IndexerAccesses { get; }

        public List<TInvocationExpressionSyntax> ArgAtInvocations { get; }

        public List<TInvocationExpressionSyntax> ArgInvocations { get; }

        public CallInfoContext(List<TInvocationExpressionSyntax> argAtInvocations, List<TInvocationExpressionSyntax> argInvocations, List<TIndexerSyntax> indexerAccesses)
        {
            IndexerAccesses = indexerAccesses;
            ArgAtInvocations = argAtInvocations;
            ArgInvocations = argInvocations;
        }
    }
}