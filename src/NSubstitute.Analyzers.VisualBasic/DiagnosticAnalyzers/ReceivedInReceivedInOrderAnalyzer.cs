using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class ReceivedInReceivedInOrderAnalyzer : AbstractReceivedInReceivedInOrderAnalyzer
{
    public ReceivedInReceivedInOrderAnalyzer()
        : base(SubstitutionNodeFinder.Instance, VisualBasic.DiagnosticDescriptorsProvider.Instance)
    {
    }
}