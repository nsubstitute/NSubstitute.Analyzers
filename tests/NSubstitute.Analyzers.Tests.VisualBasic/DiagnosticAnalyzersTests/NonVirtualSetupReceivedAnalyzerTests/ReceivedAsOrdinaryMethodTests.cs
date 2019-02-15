using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonVirtualSetupReceivedAnalyzerTests
{
    [CombinatoryData(
        "SubstituteExtensions.Received",
        "SubstituteExtensions.Received(Of Foo)",
        "SubstituteExtensions.ReceivedWithAnyArgs",
        "SubstituteExtensions.ReceivedWithAnyArgs(Of Foo)",
        "SubstituteExtensions.DidNotReceive",
        "SubstituteExtensions.DidNotReceive(Of Foo)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Foo)")]
    public class ReceivedAsOrdinaryMethodTests : NonVirtualSetupReceivedDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualMethod(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            [|{method}(substitute)|].Bar()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, NonVirtualReceivedSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualMethod(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute).Bar()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForNonSealedMethod(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public Overrides Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            {method}(substitute).Bar()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        [CombinatoryData(
            "SubstituteExtensions.Received",
            "SubstituteExtensions.Received(Of Func(Of Foo))",
            "SubstituteExtensions.ReceivedWithAnyArgs",
            "SubstituteExtensions.ReceivedWithAnyArgs(Of Func(Of Foo))",
            "SubstituteExtensions.DidNotReceive",
            "SubstituteExtensions.DidNotReceive(Of Func(Of Foo))",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Func(Of Foo))")]
        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForDelegate(string method)
        {
            var source = $@"Imports NSubstitute
Imports System

Public Class Foo
End Class

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Foo))()
            {method}(substitute)()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForSealedMethod(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Class FooBar

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class Foo
        Inherits FooBar

        Public NotOverridable Overrides Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            [|{method}(substitute)|].Bar()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, NonVirtualReceivedSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractMethod(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride Function Bar() As Integer
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute).Bar()
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceMethod(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Interface Foo

        Function Bar() As Integer

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute).Bar()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceProperty(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Interface Foo

       Property Bar As Integer

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x As Integer = {method}(substitute).Bar
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        [CombinatoryData(
            "SubstituteExtensions.Received",
            "SubstituteExtensions.Received(Of Foo(Of Integer))",
            "SubstituteExtensions.ReceivedWithAnyArgs",
            "SubstituteExtensions.ReceivedWithAnyArgs(Of Foo(Of Integer))",
            "SubstituteExtensions.DidNotReceive",
            "SubstituteExtensions.DidNotReceive(Of Foo(Of Integer))",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Foo(Of Integer))")]
        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForGenericInterfaceMethod(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Interface Foo(Of T)

        Function Bar(Of T)() As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            {method}(substitute).Bar(Of Integer)
        End Sub
    End Class
End Namespace";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractProperty(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride ReadOnly Property Bar As Integer
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            Dim x As Integer = {method}(substitute).Bar
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceIndexer(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Interface Foo

        Default Property Item(ByVal i As Integer) As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            Dim x As Integer = {method}(substitute)(1)
        End Sub
    End Class
End Namespace";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualProperty(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable ReadOnly Property Bar As Integer
            Get
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            Dim x As Integer = {method}(substitute).Bar
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualProperty(string method)
        {
            var source = $@"Imports NSubstitute

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
            Dim x As Integer = [|{method}(substitute)|].Bar
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, NonVirtualReceivedSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualIndexer(string method)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable Default Property Item(ByVal x As Integer) As Integer
            Set
                Throw New NotImplementedException
            End Set
            Get
                Throw New NotImplementedException
            End Get

        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            Dim x As Integer = {method}(substitute)(1)
        End Sub
    End Class
End Namespace";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualIndexer(string method)
        {
            var source = $@"Imports System
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
            Dim x As Integer = [|{method}(substitute)|](1)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, NonVirtualReceivedSetupSpecificationDescriptor, "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method)
        {
            var source = $@"Imports System.Runtime.CompilerServices

Namespace NSubstitute
    Public Class Foo
        Public Function Bar() As Integer
            Return 1
        End Function
    End Class

    Module SubstituteExtensions
        <Extension>
        Function Received(Of T)(ByVal returnValue As T, ByVal x as Single) As T
            Return Nothing
        End Function

        <Extension>
        Function ReceivedWithAnyArgs(Of T)(ByVal returnValue As T, ByVal x as Single) As T
            Return Nothing
        End Function

        <Extension>
        Function DidNotReceive(Of T)(ByVal returnValue As T, ByVal x as Single) As T
            Return Nothing
        End Function

        <Extension>
        Function DidNotReceiveWithAnyArgs(Of T)(ByVal returnValue As T, ByVal x as Single) As T
            Return Nothing
        End Function
    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute As Foo = Nothing
            {method}(substitute, 1D).Bar()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Friend Overridable ReadOnly Property Bar As Integer

        Friend Overridable Function FooBar() As Integer
            Return 1
        End Function

        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = [|{method}(substitute)|]{call}
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
        {
            var source = $@"Imports NSubstitute

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherFirstAssembly"")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherSecondAssembly"")>

Namespace MyNamespace
    Public Class Foo
        Friend Overridable ReadOnly Property Bar As Integer

        Friend Overridable Function FooBar() As Integer
            Return 1
        End Function

        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = {method}(substitute){call}
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
        {
            var source = $@"Imports NSubstitute

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""FirstAssembly"")>

Namespace MyNamespace
    Public Class Foo
        Friend Overridable ReadOnly Property Bar As Integer

        Friend Overridable Function FooBar() As Integer
            Return 1
        End Function

        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = [|{method}(substitute)|]{call}
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Protected Friend Overridable ReadOnly Property Bar As Integer

        Protected Friend Overridable Function FooBar() As Integer
            Return 1
        End Function

        Default Protected Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = {method}(substitute){call}
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }
    }
}