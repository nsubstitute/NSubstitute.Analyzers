using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NonSubstitutableMemberArgumentMatcherAnalyzer : AbstractNonSubstitutableMemberArgumentMatcherAnalyzer
{
    public NonSubstitutableMemberArgumentMatcherAnalyzer()
        : base(NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance)
    {
    }
}