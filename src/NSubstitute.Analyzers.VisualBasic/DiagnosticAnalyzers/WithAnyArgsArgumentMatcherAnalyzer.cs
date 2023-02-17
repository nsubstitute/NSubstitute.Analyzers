using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class WithAnyArgsArgumentMatcherAnalyzer : AbstractWithAnyArgsArgumentMatcherAnalyzer
{
    public WithAnyArgsArgumentMatcherAnalyzer()
        : base(VisualBasic.DiagnosticDescriptorsProvider.Instance, SubstitutionOperationFinder.Instance)
    {
    }
}