using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.ReceivedInReceivedInOrderCodeFixProviderTests;

public class ReceivedAsExtensionMethodTests : ReceivedInReceivedInOrderCodeFixVerifier
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
                                 substitute.Received().Bar()
                                 substitute.Received().Bar(Arg.Any(Of Integer)())
                                 substitute.ReceivedWithAnyArgs().Bar(Arg.Any(Of Integer)())
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
                                 substitute.Received(Quantity.Exactly(1)).Bar()
                                 substitute.Received(Quantity.Exactly(1)).Bar(Arg.Any(Of Integer)())
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
                             End Function)
        End Sub
    End Class
End Namespace";

        await VerifyFix(oldSource, newSource);
    }
}