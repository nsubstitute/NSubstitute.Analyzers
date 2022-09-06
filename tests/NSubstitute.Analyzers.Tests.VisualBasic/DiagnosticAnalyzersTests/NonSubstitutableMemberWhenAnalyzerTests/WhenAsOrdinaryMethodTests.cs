using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonSubstitutableMemberWhenAnalyzerTests;

[CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.When(Of Foo)", "SubstituteExtensions.WhenForAnyArgs", "SubstituteExtensions.WhenForAnyArgs(Of Foo)")]
public class WhenAsOrdinaryMethodTests : NonSubstitutableMemberWhenDiagnosticVerifier
{
    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method, string whenAction)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMemberFromBaseClass(string method, string whenAction)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class FooBar
        Public Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function
    End Class

    Public Class Foo
        Inherits FooBar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method, string whenAction)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Overridable Function Bar(ByVal x As Integer) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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
        Public Overridable Function Bar(ByVal x As Integer) As Integer
            Return 2
        End Function
    End Class

    Public Class Foo
        Inherits FooBar

        Public Overrides Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.When(Of Func(Of Integer))", "SubstituteExtensions.WhenForAnyArgs", "SubstituteExtensions.WhenForAnyArgs(Of Func(Of Integer))")]
    public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method, string whenAction)
    {
        var source = $@"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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
        Public Overridable Function Bar(ByVal x As Integer) As Integer
            Return 2
        End Function
    End Class

    Public Class Foo
        Inherits FooBar

        Public NotOverridable Overrides Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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
        Public MustOverride Function Bar(ByVal x As Integer) As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i +1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i +1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i +1)
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
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i +1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i +1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i +1)
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
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.When(Of Foo(Of Integer))", "SubstituteExtensions.WhenForAnyArgs", "SubstituteExtensions.WhenForAnyArgs(Of Foo(Of Integer))")]
    public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method, string whenAction)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Interface Foo(Of T)

        Function Bar(Of T)(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests

    Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo (Of Integer))()
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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
        Public Function Bar(ByVal x As Integer) As Integer
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
            {method}(substitute, {whenAction}, 1)
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
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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
            {method}(substitute, {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {whenAction}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {whenAction}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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
        Public Function Bar(ByVal x As Integer) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, AddressOf SubstituteCall).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= AddressOf OtherSubstituteCall).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= AddressOf YetAnotherSubstituteCall, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
            substitute.Bar(1)
        End Sub

        Private Sub SubstituteCall(ByVal [sub] As Foo)
            Dim objBarr = [|[sub].Bar(Arg.Any(Of Integer)())|]
        End Sub

        Private Sub OtherSubstituteCall(ByVal [sub] As Foo)
            Dim objBarr = [|[sub].Bar(Arg.Any(Of Integer)())|]
        End Sub

        Private Sub YetAnotherSubstituteCall(ByVal [sub] As Foo)
            Dim objBarr = [|[sub].Bar(Arg.Any(Of Integer)())|]
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
        Public Overridable Function Bar(ByVal x As Integer) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute,AddressOf SubstituteCall).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= AddressOf SubstituteCall).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= AddressOf SubstituteCall, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
            substitute.Bar(1)
        End Sub

        Private Sub SubstituteCall(ByVal [sub] As Foo)
            Dim objBarr = [sub].Bar(Arg.Any(Of Integer)())
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

        Friend Overridable Function FooBar(ByVal x As Integer) As Integer
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
            {method}(substitute, {call}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {call}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {call}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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

        Friend Overridable Function FooBar(ByVal x As Integer) As Integer
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
            {method}(substitute, {call}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {call}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {call}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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

        Friend Overridable Function FooBar(ByVal x As Integer) As Integer
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
            {method}(substitute, {call}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {call}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {call}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
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

        Protected Friend Overridable Function FooBar(ByVal x As Integer) As Integer
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
            {method}(substitute, {call}).[Do](Sub(callInfo) i = i + 1)
            {method}(substitute:= substitute, substituteCall:= {call}).[Do](Sub(callInfo) i = i + 1)
            {method}(substituteCall:= {call}, substitute:= substitute).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}