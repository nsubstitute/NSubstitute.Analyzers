using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class NonSubstitutableMemberArgumentMatcherAnalyzer : AbstractNonSubstitutableMemberArgumentMatcherAnalyzer
{
    public NonSubstitutableMemberArgumentMatcherAnalyzer()
        : base(NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance)
    {
    }
}