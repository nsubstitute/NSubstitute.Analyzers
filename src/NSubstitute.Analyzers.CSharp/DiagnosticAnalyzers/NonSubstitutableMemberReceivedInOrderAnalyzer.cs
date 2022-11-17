using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NonSubstitutableMemberReceivedInOrderAnalyzer : AbstractNonSubstitutableMemberReceivedInOrderAnalyzer
{
    public NonSubstitutableMemberReceivedInOrderAnalyzer()
        : base(SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance)
    {
    }
}