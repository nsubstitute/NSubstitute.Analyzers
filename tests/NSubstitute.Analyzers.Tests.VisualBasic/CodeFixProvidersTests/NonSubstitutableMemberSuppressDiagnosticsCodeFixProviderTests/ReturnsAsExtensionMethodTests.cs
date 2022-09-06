using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.NonSubstitutableMemberSuppressDiagnosticsCodeFixProviderTests;

public class ReturnsAsExtensionMethodTests : NonSubstitutableMemberSuppressDiagnosticsCodeFixVerifier
{
    public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualMethod()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar().Returns(1)
        End Sub
    End Class
End Namespace
";
        await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForStaticMethod()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Shared Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Foo.Bar().Returns(1)
        End Sub
    End Class
End Namespace
";
        await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForExtensionMethod()
    {
        var source = @"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Bar = NSubstitute.Substitute.[For](Of IBar)()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.GetBar().Returns(1)
        End Sub
    End Class

    Module MyExtensions
        Public Property Bar As IBar

        <Extension()>
        Function GetBar(ByVal foo As IFoo) As Integer
            Return Bar.Foo()
            Return 1
        End Function

    End Module

    Interface IBar
        Function Foo() As Integer
    End Interface

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace";

        await VerifySuppressionSettings(source, "M:MyNamespace.MyExtensions.GetBar(MyNamespace.IFoo)~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForSealedOverrideMethod()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public NotOverridable Overrides Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            substitute.Bar().Returns(1)
        End Sub
    End Class
End Namespace
";
        await VerifySuppressionSettings(source, "M:MyNamespace.Foo2.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualProperty()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public ReadOnly Property Bar As Integer
            Get
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            substitute.Bar.Returns(1)
        End Sub
    End Class
End Namespace";

        await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualIndexer()
    {
        var source = @"Imports System
Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Default ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Throw New NotImplementedException
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            substitute(1).Returns(1)
        End Sub
    End Class
End Namespace";

        await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Item(System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettingsForClass_WhenSettingsValueForNonVirtualMember_AndSelectingClassSuppression()
    {
        var source = @"Imports System
Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Default ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Throw New NotImplementedException
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            substitute(1).Returns(1)
        End Sub
    End Class
End Namespace";

        await VerifySuppressionSettings(source, "T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification, 1);
    }

    public override async Task SuppressesDiagnosticsInSettingsForNamespace_WhenSettingsValueForNonVirtualMember_AndSelectingNamespaceSuppression()
    {
        var source = @"Imports System
Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Default ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Throw New NotImplementedException
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            substitute(1).Returns(1)
        End Sub
    End Class
End Namespace";

        await VerifySuppressionSettings(source, "N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification, 2);
    }
}