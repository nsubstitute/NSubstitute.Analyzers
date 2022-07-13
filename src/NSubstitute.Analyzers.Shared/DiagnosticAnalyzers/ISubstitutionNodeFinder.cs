using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ISubstitutionNodeFinder
{
    IEnumerable<IOperation> Find(OperationAnalysisContext operationAnalysisContext, IInvocationOperation invocationOperation);

    IEnumerable<IOperation> FindForWhenExpression(OperationAnalysisContext operationAnalysisContext, IInvocationOperation invocationOperation);

    IEnumerable<IOperation> FindForReceivedInOrderExpression(OperationAnalysisContext operationAnalysisContext, IInvocationOperation invocationOperation, bool includeAll = false);

    IOperation FindForStandardExpression(IInvocationOperation invocationOperation);
}