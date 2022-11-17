using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface IReEntrantCallFinder
{
    IReadOnlyList<IOperation> GetReEntrantCalls(
        Compilation compilation,
        IInvocationOperation invocationOperation,
        IOperation rootNode);
}