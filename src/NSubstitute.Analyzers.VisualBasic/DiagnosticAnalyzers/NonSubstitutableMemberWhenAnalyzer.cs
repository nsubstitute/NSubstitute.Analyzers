using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class NonSubstitutableMemberWhenAnalyzer : AbstractNonSubstitutableMemberWhenAnalyzer
{
    public NonSubstitutableMemberWhenAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance)
    {
    }
}