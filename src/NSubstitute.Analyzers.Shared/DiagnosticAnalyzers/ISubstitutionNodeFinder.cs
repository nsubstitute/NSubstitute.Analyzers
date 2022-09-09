using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ISubstitutionNodeFinder
{
    IEnumerable<IOperation> Find(Compilation compilation, IInvocationOperation invocationOperation);

    IEnumerable<IOperation> FindForWhenExpression(Compilation compilation, IInvocationOperation invocationOperation);

    IEnumerable<IOperation> FindForReceivedInOrderExpression(Compilation compilation, IInvocationOperation invocationOperation, bool includeAll = false);

    IOperation FindForStandardExpression(IInvocationOperation invocationOperation);
}