using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.AsyncReceivedInOrderCallbackAnalyzerTests;

public class AsyncReceivedInOrderCallbackDiagnosticVerifier : VisualBasicDiagnosticVerifier, IAsyncReceivedInOrderCallbackDiagnosticVerifier
{
    private readonly DiagnosticDescriptor _descriptor = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.AsyncCallbackUsedInReceivedInOrder;

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new AsyncReceivedInOrderCallbackAnalyzer();

    [Fact]
    public async Task ReportsDiagnostic_WhenAsyncLambdaCallbackUsedInReceivedInOrder()
    {
        var source = @"Imports System.Threading.Tasks
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
         Function Bar() As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder([|Async|] Function() Await substitute.Bar())
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, _descriptor);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenAsyncDelegateCallbackUsedInReceivedInOrder()
    {
        var source = @"Imports System.Threading.Tasks
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
         ReadOnly Property Bar As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder([|Async|] Function()
                                 Dim x = Await substitute.Bar
                             End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, _descriptor);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenNonAsyncLambdaCallbackUsedInReceivedInOrder()
    {
        var source = @"Imports System.Threading.Tasks
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
         Function Bar() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder(Function() substitute.Bar())
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenNonAsyncDelegateCallbackUsedInReceivedInOrder()
    {
        var source = @"Imports System.Threading.Tasks
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
         ReadOnly Property Bar As Integer 
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder(Function()
                                 Dim x = substitute.Bar
                             End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod()
    {
        var source = @"Imports System
Imports System.Threading.Tasks

Namespace MyNamespace
    Interface IFoo
        ReadOnly Property Bar As Task(Of Integer)
    End Interface

    Public Class Received
        Public Shared Sub InOrder(ByVal action As Action)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute As IFoo = Nothing
            Received.InOrder(Async Sub()
                                 Dim x = Await substitute.Bar
                             End Sub)
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }
}