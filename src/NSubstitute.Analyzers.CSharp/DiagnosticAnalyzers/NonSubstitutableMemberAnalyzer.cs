using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NonSubstitutableMemberAnalyzer : AbstractNonSubstitutableMemberAnalyzer
{
    public NonSubstitutableMemberAnalyzer()
        : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance, SubstitutionOperationFinder.Instance, NonSubstitutableMemberAnalysis.Instance)
    {
    }
}