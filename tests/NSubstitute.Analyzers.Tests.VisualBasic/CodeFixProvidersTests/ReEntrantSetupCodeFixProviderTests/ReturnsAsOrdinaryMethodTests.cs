using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.ReEntrantSetupCodeFixProviderTests;

public class ReturnsAsOrdinaryMethodTests : ReEntrantSetupCodeFixVerifier
{
    public override async Task ReplacesArgumentExpression_WithLambda(string arguments, string rewrittenArguments)
    {
        var oldSource = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Private firstSubstitute As IFoo = Substitute.[For](Of IFoo)()

        Public Sub New()
            firstSubstitute.Id.Returns(45)
        End Sub

        Interface IFoo
            ReadOnly Property Id As Integer
        End Interface

        Public Sub Test()
            Dim secondSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.Returns(secondSubstitute.Id, {arguments})
        End Sub

        Private Function CreateReEntrantSubstitute() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Id.Returns(1)
            Return 1
        End Function

        Private Function CreateDefaultValue() As Integer
            Return 1
        End Function
    End Class
End Namespace
";

        var newSource = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Private firstSubstitute As IFoo = Substitute.[For](Of IFoo)()

        Public Sub New()
            firstSubstitute.Id.Returns(45)
        End Sub

        Interface IFoo
            ReadOnly Property Id As Integer
        End Interface

        Public Sub Test()
            Dim secondSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.Returns(secondSubstitute.Id, {rewrittenArguments})
        End Sub

        Private Function CreateReEntrantSubstitute() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Id.Returns(1)
            Return 1
        End Function

        Private Function CreateDefaultValue() As Integer
            Return 1
        End Function
    End Class
End Namespace
";
        await VerifyFix(oldSource, newSource);
    }

    public override async Task ReplacesArgumentExpression_WithLambdaWithReducedTypes_WhenGeneratingArrayParamsArgument()
    {
        var oldSource = @"Imports NSubstitute
Imports NSubstitute.Core
Imports System

Namespace MyNamespace
    Public Class FooTests
        Private firstSubstitute As IFoo = Substitute.[For](Of IFoo)()
        Public Shared Property Value As Integer

        Public Sub New()
            firstSubstitute.Id.Returns(45)
        End Sub

        Interface IFoo
            ReadOnly Property Id As Integer
        End Interface

        Public Sub Test()
            Dim secondSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.Returns(secondSubstitute.Id, CreateReEntrantSubstitute(), {MyNamespace.FooTests.Value})
        End Sub

        Private Function CreateReEntrantSubstitute() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Id.Returns(1)
            Return 1
        End Function
    End Class
End Namespace
";

        var newSource = @"Imports NSubstitute
Imports NSubstitute.Core
Imports System

Namespace MyNamespace
    Public Class FooTests
        Private firstSubstitute As IFoo = Substitute.[For](Of IFoo)()
        Public Shared Property Value As Integer

        Public Sub New()
            firstSubstitute.Id.Returns(45)
        End Sub

        Interface IFoo
            ReadOnly Property Id As Integer
        End Interface

        Public Sub Test()
            Dim secondSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.Returns(secondSubstitute.Id, Function(x) CreateReEntrantSubstitute(), New Func(Of CallInfo(Of Integer), Integer)() {Function(x) MyNamespace.FooTests.Value})
        End Sub

        Private Function CreateReEntrantSubstitute() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Id.Returns(1)
            Return 1
        End Function
    End Class
End Namespace
";
        await VerifyFix(oldSource, newSource);
    }

    public override async Task ReplacesArgumentExpression_WithLambdaWithNonGenericCallInfo_WhenGeneratingArrayParamsArgument()
    {
        var oldSource = @"Imports NSubstitute
Imports NSubstitute.Core
Imports System

Namespace MyNamespace
    Public Class FooTests
        Private firstSubstitute As IFoo = Substitute.[For](Of IFoo)()
        Public Shared Property Value As Integer

        Public Sub New()
            firstSubstitute.Id.Returns(45)
        End Sub

        Interface IFoo
            ReadOnly Property Id As Integer
        End Interface

        Public Sub Test()
            Dim secondSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.Returns(secondSubstitute.Id, CreateReEntrantSubstitute(), {MyNamespace.FooTests.Value})
        End Sub

        Private Function CreateReEntrantSubstitute() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Id.Returns(1)
            Return 1
        End Function
    End Class
End Namespace
";

        var newSource = @"Imports NSubstitute
Imports NSubstitute.Core
Imports System

Namespace MyNamespace
    Public Class FooTests
        Private firstSubstitute As IFoo = Substitute.[For](Of IFoo)()
        Public Shared Property Value As Integer

        Public Sub New()
            firstSubstitute.Id.Returns(45)
        End Sub

        Interface IFoo
            ReadOnly Property Id As Integer
        End Interface

        Public Sub Test()
            Dim secondSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.Returns(secondSubstitute.Id, Function(x) CreateReEntrantSubstitute(), New Func(Of CallInfo, Integer)() {Function(x) MyNamespace.FooTests.Value})
        End Sub

        Private Function CreateReEntrantSubstitute() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Id.Returns(1)
            Return 1
        End Function
    End Class
End Namespace
";
        await VerifyFix(oldSource, newSource, version: NSubstituteVersion.NSubstitute4_2_2);
    }
}