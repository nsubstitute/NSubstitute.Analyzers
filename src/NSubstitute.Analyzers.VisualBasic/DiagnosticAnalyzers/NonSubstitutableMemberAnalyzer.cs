using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class NonSubstitutableMemberAnalyzer : AbstractNonSubstitutableMemberAnalyzer
{
    public NonSubstitutableMemberAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance)
    {
    }
}