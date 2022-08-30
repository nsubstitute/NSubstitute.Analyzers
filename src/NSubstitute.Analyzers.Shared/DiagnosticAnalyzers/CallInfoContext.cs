using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal class CallInfoContext
{
    public static CallInfoContext Empty { get; } = new(
        Array.Empty<IInvocationOperation>(),
        Array.Empty<IInvocationOperation>(),
        Array.Empty<IOperation>());

    public IReadOnlyList<IOperation> IndexerAccessesOperations { get; }

    public IReadOnlyList<IInvocationOperation> ArgAtInvocationsOperations { get; }

    public IReadOnlyList<IInvocationOperation> ArgInvocationsOperations { get; }

    public CallInfoContext(
        IReadOnlyList<IInvocationOperation> argAtInvocations,
        IReadOnlyList<IInvocationOperation> argInvocations,
        IReadOnlyList<IOperation> indexerAccesses)
    {
        IndexerAccessesOperations = indexerAccesses;
        ArgAtInvocationsOperations = argAtInvocations;
        ArgInvocationsOperations = argInvocations;
    }

    public CallInfoContext Merge(CallInfoContext callInfoContext)
    {
        return new CallInfoContext(
            ArgAtInvocationsOperations.Concat(callInfoContext.ArgAtInvocationsOperations).ToList(),
            ArgInvocationsOperations.Concat(callInfoContext.ArgInvocationsOperations).ToList(),
            IndexerAccessesOperations.Concat(callInfoContext.IndexerAccessesOperations).ToList());
    }
}