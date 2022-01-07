using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.NonSubstitutableMemberReceivedInOrderSuppressDiagnosticsCodeFixProviderTests;

public class NonSubstitutableMemberReceivedInOrderSuppressDiagnosticsCodeFixVerifier : VisualBasicSuppressDiagnosticSettingsVerifier, INonSubstitutableMemberSuppressDiagnosticsCodeFixVerifier
{
    protected override CodeFixProvider CodeFixProvider { get; } = new NonSubstitutableMemberSuppressDiagnosticsCodeFixProvider();

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberReceivedInOrderAnalyzer();

    [Fact]
    public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualMethod()
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
            NSubstitute.Received.InOrder(Sub()  substitute.Bar())
        End Sub
    End Class
End Namespace
";
        await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
    }

    [Fact]
    public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForStaticMethod()
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
            NSubstitute.Received.InOrder(Sub() Foo.Bar())
        End Sub
    End Class
End Namespace
";
        await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
    }

    [Fact]
    public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForExtensionMethod()
    {
        var source = @"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Bar = NSubstitute.Substitute.[For](Of IBar)()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder(Sub()  substitute.GetBar())
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

        await VerifySuppressionSettings(source, "M:MyNamespace.MyExtensions.GetBar(MyNamespace.IFoo)~System.Int32", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
    }

    [Fact]
    public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForSealedOverrideMethod()
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
            NSubstitute.Received.InOrder(Function()
                                             Dim x = substitute.Bar()
                                             Return 1
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifySuppressionSettings(source, "M:MyNamespace.Foo2.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
    }

    [Fact]
    public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualProperty()
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
            NSubstitute.Received.InOrder(Function()
                                             Dim x = substitute.Bar
                                             Return 1
                                         End Function)
        End Sub
    End Class
End Namespace";

        await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
    }

    [Fact]
    public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualIndexer()
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
            NSubstitute.Received.InOrder(Function()
                                             Dim x = substitute(1)
                                             Return 1
                                         End Function)
        End Sub
    End Class
End Namespace";

        await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Item(System.Int32)", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
    }

    [Fact]
    public async Task SuppressesDiagnosticsInSettingsForClass_WhenSettingsValueForNonVirtualMember_AndSelectingClassSuppression()
    {
        var source = @"Imports System
Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Default ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             Dim x = substitute(1)
                                             Return 1
                                         End Function)
        End Sub
    End Class
End Namespace";

        await VerifySuppressionSettings(source, "T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification, 1);
    }

    [Fact]
    public async Task SuppressesDiagnosticsInSettingsForNamespace_WhenSettingsValueForNonVirtualMember_AndSelectingNamespaceSuppression()
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
            NSubstitute.Received.InOrder(Function()
                                             Dim x = substitute(1)
                                             Return 1
                                         End Function)
        End Sub
    End Class
End Namespace";

        await VerifySuppressionSettings(source, "N:MyNamespace", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification, 2);
    }
}