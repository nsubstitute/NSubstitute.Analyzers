using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal class CallInfoContext
    {
        public IReadOnlyList<SyntaxNode> IndexerAccesses { get; }

        public IReadOnlyList<SyntaxNode> ArgAtInvocations { get; }

        public IReadOnlyList<SyntaxNode> ArgInvocations { get; }

        public CallInfoContext(
            IReadOnlyList<SyntaxNode> argAtInvocations,
            IReadOnlyList<SyntaxNode> argInvocations,
            IReadOnlyList<SyntaxNode> indexerAccesses)
        {
            IndexerAccesses = indexerAccesses;
            ArgAtInvocations = argAtInvocations;
            ArgInvocations = argInvocations;
        }
    }
}