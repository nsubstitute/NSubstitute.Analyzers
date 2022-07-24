using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface IReEntrantCallFinder
{
    ImmutableList<IInvocationOperation> GetReEntrantCalls(
        Compilation compilation,
        IInvocationOperation invocationOperation,
        IOperation rootNode);
}