using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.ReceivedInReceivedInOrderCodeFixProviderTests;

public class ReceivedAsOrdinaryMethodTests : ReceivedInReceivedInOrderCodeFixVerifier
{
    [Fact]
    public override async Task RemovesReceivedChecks_WhenReceivedChecksHasNoArguments()
    {
        var oldSource = @"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder(Function()
                                 SubstituteExtensions.Received(substitute).Bar()
                                 SubstituteExtensions.Received(substitute).Bar(Arg.Any(Of Integer)())
                                 SubstituteExtensions.ReceivedWithAnyArgs(substitute).Bar(Arg.Any(Of Integer)())
                             End Function)
        End Sub
    End Class
End Namespace";

        var newSource = @"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder(Function()
                                 substitute.Bar()
                                 substitute.Bar(Arg.Any(Of Integer)())
                                 substitute.Bar(Arg.Any(Of Integer)())
                             End Function)
        End Sub
    End Class
End Namespace";

        await VerifyFix(oldSource, newSource);
    }

    [Fact]
    public override async Task RemovesReceivedChecks_WhenReceivedChecksHasArguments()
    {
        var oldSource = @"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder(Function()
                                 SubstituteExtensions.Received(substitute, 1).Bar()
                                 SubstituteExtensions.Received(substitute:= substitute, requiredNumberOfCalls:= 1).Bar()
                                 SubstituteExtensions.Received(requiredNumberOfCalls:= 1, substitute:= substitute).Bar()
                                 SubstituteExtensions.Received(substitute, 1).Bar(Arg.Any(Of Integer)())
                                 SubstituteExtensions.Received(substitute:= substitute, requiredNumberOfCalls:= 1).Bar(Arg.Any(Of Integer)())
                                 SubstituteExtensions.Received(requiredNumberOfCalls:= 1, substitute:= substitute).Bar(Arg.Any(Of Integer)())
                             End Function)
        End Sub
    End Class
End Namespace";

        var newSource = @"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder(Function()
                                 substitute.Bar()
                                 substitute.Bar()
                                 substitute.Bar()
                                 substitute.Bar(Arg.Any(Of Integer)())
                                 substitute.Bar(Arg.Any(Of Integer)())
                                 substitute.Bar(Arg.Any(Of Integer)())
                             End Function)
        End Sub
    End Class
End Namespace";

        await VerifyFix(oldSource, newSource);
    }
}