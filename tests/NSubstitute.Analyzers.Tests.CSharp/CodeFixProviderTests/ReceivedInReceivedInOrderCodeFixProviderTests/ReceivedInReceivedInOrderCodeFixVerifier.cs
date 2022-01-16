using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.ReceivedInReceivedInOrderCodeFixProviderTests;

public abstract class ReceivedInReceivedInOrderCodeFixVerifier : CSharpCodeFixVerifier, IReceivedInReceivedInOrderCodeFixVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new ReceivedInReceivedInOrderAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new ReceivedInReceivedInOrderCodeFixProvider();

    public abstract Task RemovesReceivedChecks_WhenReceivedChecksHasNoArguments();

    public abstract Task RemovesReceivedChecks_WhenReceivedChecksHasArguments();
}