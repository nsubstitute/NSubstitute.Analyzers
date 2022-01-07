using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface INonSubstitutableMemberAnalysis
{
    NonSubstitutableMemberAnalysisResult Analyze(
        in SyntaxNodeAnalysisContext syntaxNodeContext,
        SyntaxNode accessedMember,
        ISymbol symbol = null);
}