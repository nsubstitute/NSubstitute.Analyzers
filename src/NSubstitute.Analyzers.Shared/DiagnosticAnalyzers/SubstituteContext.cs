using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal struct SubstituteContext
{
    public OperationAnalysisContext OperationAnalysisContext { get; }

    public IInvocationOperation InvocationOperation { get; }

    public SubstituteContext(OperationAnalysisContext operationAnalysisContext, IInvocationOperation invocationOperation)
    {
        OperationAnalysisContext = operationAnalysisContext;
        InvocationOperation = invocationOperation;
    }
}