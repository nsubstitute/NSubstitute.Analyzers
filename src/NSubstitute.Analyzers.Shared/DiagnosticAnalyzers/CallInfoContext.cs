using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal class CallInfoContext
{
    public static CallInfoContext Empty { get; } = new CallInfoContext(
        Array.Empty<SyntaxNode>(),
        Array.Empty<SyntaxNode>(),
        Array.Empty<SyntaxNode>());

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

    public CallInfoContext Merge(CallInfoContext callInfoContext)
    {
        return new CallInfoContext(
            ArgAtInvocations.Concat(callInfoContext.ArgAtInvocations).ToList(),
            ArgInvocations.Concat(callInfoContext.ArgInvocations).ToList(),
            IndexerAccesses.Concat(callInfoContext.IndexerAccesses).ToList());
    }
}