using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonSubstitutableMemberAnalyzerTests;

[CombinatoryData("ExceptionExtensions.Throws", "ExceptionExtensions.ThrowsForAnyArgs")]
public class ThrowsAsOrdinaryMethodTests : NonSubstitutableMemberDiagnosticVerifier
{
    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(value:= [|substitute.Bar()|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute.Bar()|])
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForLiteral(string method, string literal, string type)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class FooTests
        Public Sub Test()
            {method}([|{literal}|], New Exception())
            {method}(value:= [|{literal}|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|{literal}|])
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, $"Member {literal} can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForStaticMethod(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Shared Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            {method}([|Foo.Bar()|], New Exception())
            {method}(value:= [|Foo.Bar()|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|Foo.Bar()|])
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(), New Exception())
            {method}(value:= substitute.Bar(), ex:= New Exception())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

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
            {method}(substitute.Bar(), New Exception())
            {method}(value:= substitute.Bar(), ex:= New Exception())
            {method}(ex:= New Exception(), value:= substitute.Bar())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim returnValue = substitute.Bar()
            {method}(returnValue, New Exception())
            {method}(value:= returnValue, ex:= New Exception())
            {method}(ex:= New Exception(), value:= returnValue)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            {method}(substitute(), New Exception())
            {method}(value:= substitute(), ex:= New Exception())
            {method}(ex:= New Exception(), value:= substitute())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

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
            {method}([|substitute.Bar()|], New Exception())
            {method}(value:= [|substitute.Bar()|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute.Bar()|])
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride Function Bar() As Integer
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(), New Exception())
            {method}(value:= substitute.Bar(), ex:= New Exception())
            {method}(ex:= New Exception(), value:= substitute.Bar())
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Interface IFoo

        Function Bar() As Integer

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            {method}(substitute.Bar(), New Exception())
            {method}(value:= substitute.Bar(), ex:= New Exception())
            {method}(ex:= New Exception(), value:= substitute.Bar())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Interface IFoo

       Property Bar As Integer

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            {method}(substitute.Bar, New Exception())
            {method}(value:= substitute.Bar, ex:= New Exception())
            {method}(ex:= New Exception(), value:= substitute.Bar)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Interface IFoo(Of T)

        Function Bar(Of T)() As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo(Of Integer))()
            {method}(substitute.Bar(Of Integer), New Exception())
            {method}(value:= substitute.Bar(Of Integer), ex:= New Exception())
            {method}(ex:= New Exception(), value:= substitute.Bar(Of Integer))
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride ReadOnly Property Bar As Integer
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            {method}(substitute.Bar, New Exception())
            {method}(value:= substitute.Bar, ex:= New Exception())
            {method}(ex:= New Exception(), value:= substitute.Bar)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace

    Public Interface IFoo

        Default Property Item(ByVal i As Integer) As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of IFoo)
            {method}(substitute(1), New Exception())
            {method}(value:= substitute(1), ex:= New Exception())
            {method}(ex:= New Exception(), value:= substitute(1))
        End Sub
    End Class
End Namespace";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

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
            {method}(substitute.Bar, New Exception())
            {method}(value:= substitute.Bar, ex:= New Exception())
            {method}(ex:= New Exception(), value:= substitute.Bar)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

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
            {method}([|substitute.Bar|], New Exception())
            {method}(value:= [|substitute.Bar|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute.Bar|])
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

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
            {method}(substitute(1), New Exception())
            {method}(value:= substitute(1), ex:= New Exception())
            {method}(ex:= New Exception(), value:= substitute(1))
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

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
            {method}([|substitute(1)|], New Exception())
            {method}(value:= [|substitute(1)|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute(1)|])
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method)
    {
        var source = $@"Imports System.Runtime.CompilerServices
Imports System
Namespace NSubstitute
    Public Class Foo
        Public Function Bar() As Integer
            Return 1
        End Function
    End Class

    Module ExceptionExtensions
        <Extension>
        Function Throws(Of T)(ByVal returnValue As T, ex As Exception) As T
            Return Nothing
        End Function

        <Extension>
        Function ThrowsForAnyArgs(Of T)(ByVal returnValue As T, ex As Exception) As T
            Return Nothing
        End Function
    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute As Foo = Nothing
            {method}(substitute.Bar(), New Exception())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Public ReadOnly Property Bar As Integer
        Public ReadOnly Property FooBar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar, New Exception())
            {method}([|substitute.FooBar|], New Exception())
            {method}(value:= [|substitute.FooBar|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute.FooBar|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Public ReadOnly Property Bar As T
        Public ReadOnly Property FooBar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            {method}(substitute.Bar, New Exception())
            {method}([|substitute.FooBar|], New Exception())
            {method}(value:= [|substitute.FooBar|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute.FooBar|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function

        Public Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(1, 2), New Exception())
            {method}([|substitute.Bar(1)|], New Exception())
            {method}(value:= [|substitute.Bar(1)|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute.Bar(1)|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function

        Public Function Bar(Of T)(ByVal x As T, ByVal y As T) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(Of Integer)(1, 2), New Exception())
            {method}([|substitute.Bar(1)|], New Exception())
            {method}(value:= [|substitute.Bar(1)|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute.Bar(1)|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute(1 ,2), New Exception())
            {method}([|substitute(1)|], New Exception())
            {method}(value:= [|substitute(1)|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute(1)|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Default Public ReadOnly Property Item(ByVal x As T) As Integer
            Get
                Return 0
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As T, ByVal y As T) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            {method}(substitute(1 ,2), New Exception())
            {method}([|substitute(1)|], New Exception())
            {method}(value:= [|substitute(1)|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute(1)|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooBarBar
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute(1), New Exception())
            {method}(substitute.Bar, New Exception())
            {method}(substitute.FooBar(), New Exception())
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()

            {method}([|substituteFooBarBar(1)|], New Exception())
            {method}([|substituteFooBarBar.Bar|], New Exception())
            {method}([|substituteFooBarBar.FooBar()|], New Exception())
        End Sub
    End Class
End Namespace
";

        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooBarBar(Of T)
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            {method}(substitute(1), New Exception())
            {method}(substitute.Bar, New Exception())
            {method}(substitute.FooBar(), New Exception())
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar(Of Integer))()

            {method}([|substituteFooBarBar(1)|], New Exception())
            {method}([|substituteFooBarBar.Bar|], New Exception())
            {method}([|substituteFooBarBar.FooBar()|], New Exception())
        End Sub
    End Class
End Namespace
";

        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports MyOtherNamespace

Namespace MyOtherNamespace
    Public Class FooBarBar
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class
End Namespace

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute(1), New Exception())
            {method}(substitute.Bar, New Exception())
            {method}(substitute.FooBar(), New Exception())
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()

            {method}([|substituteFooBarBar(1)|], New Exception())
            {method}([|substituteFooBarBar.Bar|], New Exception())
            {method}([|substituteFooBarBar.FooBar()|], New Exception())
        End Sub
    End Class
End Namespace
";

        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(MyNamespace.IFoo)~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Bar = NSubstitute.Substitute.[For](Of IBar)()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            {method}(substitute.GetBar(), New Exception())
            {method}([|substitute.GetFooBar()|], New Exception())
            {method}(value:= [|substitute.GetFooBar()|], ex:= New Exception())
            {method}(ex:= New Exception(), value:= [|substitute.GetFooBar()|])
        End Sub
    End Class

    Module MyExtensions
        Public Property Bar As IBar

        <Extension()>
        Function GetBar(ByVal foo As IFoo) As Integer
            Return Bar.Foo()
            Return 1
        End Function

        <Extension()>
        Function GetFooBar(ByVal foo As IFoo) As Integer
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

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member GetFooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Friend Overridable ReadOnly Property Bar As Foo

        Friend Overridable Function FooBar() As Foo
            Return Nothing
        End Function

        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = {method}([|substitute{call}|], New Exception())
            Dim y = {method}(value:= [|substitute{call}|], ex:= New Exception())
            Dim z = {method}(ex:= New Exception(), value:= [|substitute{call}|])
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherFirstAssembly"")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherSecondAssembly"")>

Namespace MyNamespace
    Public Class Foo
        Friend Overridable ReadOnly Property Bar As Foo

        Friend Overridable Function FooBar() As Foo
            Return Nothing
        End Function

        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = {method}(substitute{call}, New Exception())
            Dim y = {method}(value:= substitute{call}, ex:= New Exception())
            Dim z = {method}(ex:= New Exception(), value:= substitute{call})
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""FirstAssembly"")>

Namespace MyNamespace
    Public Class Foo
        Friend Overridable ReadOnly Property Bar As Foo

        Friend Overridable Function FooBar() As Foo
            Return Nothing
        End Function

        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = {method}([|substitute{call}|], New Exception())
            Dim y = {method}(value:= [|substitute{call}|], ex:= New Exception())
            Dim z = {method}(ex:= New Exception(), value:= [|substitute{call}|])
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public Class Foo
        Protected Friend Overridable ReadOnly Property Bar As Foo

        Protected Friend Overridable Function FooBar() As Foo
            Return Nothing
        End Function

        Default Protected Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = {method}(substitute{call}, New Exception())
            Dim y = {method}(value:= substitute{call}, ex:= New Exception())
            Dim z = {method}(ex:= New Exception(), value:= substitute{call})
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}