using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ICallInfoFinder
{
    CallInfoContext GetCallInfoContext(IArgumentOperation argumentOperation);
}