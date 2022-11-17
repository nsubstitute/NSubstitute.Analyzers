using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class NonSubstitutableMemberReceivedAnalyzer : AbstractNonSubstitutableMemberReceivedAnalyzer
{
    public NonSubstitutableMemberReceivedAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, NonSubstitutableMemberAnalysis.Instance)
    {
    }
}