using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface INonSubstitutableMemberAnalysis
{
    NonSubstitutableMemberAnalysisResult Analyze(IOperation operation);
}