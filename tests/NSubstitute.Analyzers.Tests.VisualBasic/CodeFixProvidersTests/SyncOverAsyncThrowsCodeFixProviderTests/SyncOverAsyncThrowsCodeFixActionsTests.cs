using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.SyncOverAsyncThrowsCodeFixProviderTests;

public class SyncOverAsyncThrowsCodeFixActionsTests : VisualBasicCodeFixActionsVerifier, ISyncOverAsyncThrowsCodeFixActionsVerifier
{
    protected override CodeFixProvider CodeFixProvider { get; } = new SyncOverAsyncThrowsCodeFixProvider();

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SyncOverAsyncThrowsAnalyzer();

    [Theory]
    [InlineData("Throws", "Replace with Returns")]
    [InlineData("ThrowsForAnyArgs", "Replace with ReturnsForAnyArgs")]
    public async Task CreatesCodeAction_WhenOverloadSupported(string method, string expectedCodeActionTitle)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}(New Exception())
        End Sub
    End Class
End Namespace";

        await VerifyCodeActions(source, expectedCodeActionTitle);
    }

    [Theory]
    [InlineData("Throws")]
    [InlineData("ThrowsForAnyArgs")]
    public async Task DoesNotCreateCodeAction_WhenOverloadNotSupported(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}(function(info) New Exception())
            ExceptionExtensions.{method}(substitute.Bar(), function(info) New Exception())
        End Sub
    End Class
End Namespace";

        await VerifyCodeActions(source);
    }
}