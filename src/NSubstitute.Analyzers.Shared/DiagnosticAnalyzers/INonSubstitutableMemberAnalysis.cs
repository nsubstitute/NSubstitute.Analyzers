using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface INonSubstitutableMemberAnalysis
{
    NonSubstitutableMemberAnalysisResult Analyze(
        in SyntaxNodeAnalysisContext syntaxNodeContext,
        SyntaxNode accessedMember,
        ISymbol symbol = null);

    NonSubstitutableMemberAnalysisResult Analyze(
        IInvocationOperation invocationOperation,
        ISymbol symbol = null);

    NonSubstitutableMemberAnalysisResult Analyze(IOperation operation);
}