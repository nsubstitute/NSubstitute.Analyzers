using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.ReceivedInReceivedInOrderCodeFixProviderTests;

public abstract class ReceivedInReceivedInOrderCodeFixVerifier : VisualBasicCodeFixVerifier, IReceivedInReceivedInOrderCodeFixVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new ReceivedInReceivedInOrderAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new ReceivedInReceivedInOrderCodeFixProvider();

    public abstract Task RemovesReceivedChecks_WhenReceivedChecksHasNoArguments();

    public abstract Task RemovesReceivedChecks_WhenReceivedChecksHasArguments();
}