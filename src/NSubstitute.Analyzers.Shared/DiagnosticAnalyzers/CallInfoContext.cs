using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal class CallInfoContext<TInvocationExpressionSyntax, TIndexerSyntax>
        where TInvocationExpressionSyntax : SyntaxNode where TIndexerSyntax : SyntaxNode
    {
        public IReadOnlyList<TIndexerSyntax> IndexerAccesses { get; }

        public IReadOnlyList<TInvocationExpressionSyntax> ArgAtInvocations { get; }

        public IReadOnlyList<TInvocationExpressionSyntax> ArgInvocations { get; }

        public static CallInfoContext<TInvocationExpressionSyntax, TIndexerSyntax> Empty { get; } =
            new CallInfoContext<TInvocationExpressionSyntax, TIndexerSyntax>(
                Array.Empty<TInvocationExpressionSyntax>(),
                Array.Empty<TInvocationExpressionSyntax>(),
                Array.Empty<TIndexerSyntax>());

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