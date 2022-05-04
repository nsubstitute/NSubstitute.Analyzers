using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ICallInfoFinder
{
    CallInfoContext GetCallInfoContext(SemanticModel semanticModel, IArgumentOperation argumentOperation);
}