using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class ReceivedInReceivedInOrderAnalyzer : AbstractReceivedInReceivedInOrderAnalyzer
{
    public ReceivedInReceivedInOrderAnalyzer()
        : base(SubstitutionOperationFinder.Instance, CSharp.DiagnosticDescriptorsProvider.Instance)
    {
    }
}