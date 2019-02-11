using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonVirtualSetupWhenAnalyzerTests
{
    [CombinatoryData("When", "WhenForAnyArgs")]
    public class WhenAsExtensionMethodTests : NonVirtualSetupWhenDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method, string whenAction)
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
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method, string whenAction)
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
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method, string whenAction)
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

        Public Overrides Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method, string whenAction)
        {
            var source = $@"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method, string whenAction)
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
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method, string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar() As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i +1)
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method, string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i +1)
        End Sub
    End Class
End Namespace";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method, string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        ReadOnly Property Bar As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method, string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Interface Foo(Of T)

        Function Bar(Of T)() As Integer
    End Interface

    Public Class FooTests

    Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo (Of Integer))()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method, string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride ReadOnly Property Bar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method, string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Default Property Item(ByVal i As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method, string whenAction)
        {
            var source = $@"Imports System.Runtime.CompilerServices

Namespace NSubstitute
    Public Class Foo
        Public Function Bar() As Integer
            Return 1
        End Function
    End Class

    Module SubstituteExtensions
        <Extension()>
        Function [When](Of T)(ByVal substitute As T, ByVal substituteCall As System.Action(Of T), ByVal x As Integer) As T
            Return Nothing
        End Function

    <Extension()>
        Function [WhenForAnyArgs](Of T)(ByVal substitute As T, ByVal substituteCall As System.Action(Of T), ByVal x As Integer) As T
            Return Nothing
        End Function

    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute As Foo = Nothing
            substitute.{method}({whenAction}, 1)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method, string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public ReadOnly Property Bar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method, string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public Overridable Property Bar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method, string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
     Public Class Foo

        Public Default ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Throw New System.NotImplementedException
            End Get
        End Property
    End Class


    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction(string method)
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
            Dim i As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}(AddressOf SubstituteCall).[Do](Sub(callInfo) i = i + 1)
            substitute.Bar()
        End Sub

        Private Sub SubstituteCall(ByVal [sub] As Foo)
            Dim objBarr = [|[sub].Bar()|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction(string method)
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
            Dim i As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}(AddressOf SubstituteCall).[Do](Sub(callInfo) i = i + 1)
            substitute.Bar()
        End Sub

        Private Sub SubstituteCall(ByVal [sub] As Foo)
            Dim objBarr = [sub].Bar()
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
            Dim i As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({call}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, InternalWhenSetupSpecificationDescriptor, message);
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
            Dim i As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({call}).[Do](Sub(callInfo) i = i + 1)
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
            Dim i As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({call}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, InternalWhenSetupSpecificationDescriptor, message);
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
            Dim i As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}({call}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }
    }
}