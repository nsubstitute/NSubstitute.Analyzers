using System.Linq;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonSubstitutableMemberAnalyzerTests;

[CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns(Of Integer)", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)")]
public class ReturnsAsOrdinaryMethodTests : NonSubstitutableMemberDiagnosticVerifier
{
    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method)
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
            {method}([|substitute.Bar()|], 1)
            {method}(value:= [|substitute.Bar()|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute.Bar()|])
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    [CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns(Of)", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs(Of)")]
    public override async Task ReportsDiagnostics_WhenUsedWithLiteral(string method, string literal, string type)
    {
        method = method.Replace("(Of)", $"(Of {type})");

        var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Class FooTests
        Public Sub Test()
            {method}([|{literal}|], {literal})
            {method}(value:= [|{literal}|], returnThis:= {literal})
            {method}(returnThis:= {literal}, value:= [|{literal}|])
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, $"Member {literal} can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenUsedWithStaticMethod(string method)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Shared Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            {method}([|Foo.Bar()|], 1)
            {method}(value:= [|Foo.Bar()|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|Foo.Bar()|])
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method)
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
            {method}(substitute.Bar(), 1)
            {method}(value:= substitute.Bar(), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithNonSealedOverrideMethod(string method)
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
            {method}(substitute.Bar(), 1)
            {method}(value:= substitute.Bar(), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method)
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
            Dim returnValue = substitute.Bar()
            {method}(returnValue, 1)
            {method}(value:= returnValue, returnThis:= 1)
            {method}(returnThis:= 1, value:= returnValue)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method)
    {
        var source = $@"Imports NSubstitute
Imports System

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            {method}(substitute(), 1)
            {method}(value:= substitute(), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method)
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

        Public NotOverridable Overrides Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            {method}([|substitute.Bar()|], 1)
            {method}(value:= [|substitute.Bar()|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute.Bar()|])
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride Function Bar() As Integer
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(), 1)
            {method}(value:= substitute.Bar(), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar())
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceMethod(string method)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace

    Interface IFoo

        Function Bar() As Integer

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            {method}(substitute.Bar(), 1)
            {method}(value:= substitute.Bar(), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceProperty(string method)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace

    Interface IFoo

       Property Bar As Integer

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            {method}(substitute.Bar, 1)
            {method}(value:= substitute.Bar, returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Interface IFoo(Of T)

        Function Bar(Of T)() As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo(Of Integer))()
            {method}(substitute.Bar(Of Integer), 1)
            {method}(value:= substitute.Bar(Of Integer), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar(Of Integer))
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride ReadOnly Property Bar As Integer
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            {method}(substitute.Bar, 1)
            {method}(value:= substitute.Bar, returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Interface IFoo

        Default Property Item(ByVal i As Integer) As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of IFoo)
            {method}(substitute(1), 1)
            {method}(value:= substitute(1), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute(1))
        End Sub
    End Class
End Namespace";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method)
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
            {method}(substitute.Bar, 1)
            {method}(value:= substitute.Bar, returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method)
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
            {method}([|substitute.Bar|], 1)
            {method}(value:= [|substitute.Bar|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute.Bar|])
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualIndexer(string method)
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
            {method}(substitute(1), 1)
            {method}(value:= substitute(1), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute(1))
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method)
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
            {method}([|substitute(1)|], 1)
            {method}(value:= [|substitute(1)|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute(1)|])
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
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
        Function Returns(Of T)(ByVal returnValue As T, ByVal returnThis As T) As T
            Return Nothing
        End Function

        <Extension>
        Function ReturnsForAnyArgs(Of T)(ByVal returnValue As T, ByVal returnThis As T) As T
            Return Nothing
        End Function
    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute As Foo = Nothing
            {method}(substitute.Bar(), 1)
            {method}(returnValue:= substitute.Bar(), returnThis:= 1)
            {method}(returnThis:= 1, returnValue:= substitute.Bar())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public ReadOnly Property Bar As Integer
        Public ReadOnly Property FooBar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar, 1)
            {method}([|substitute.FooBar|], 1)
            {method}(value:= [|substitute.FooBar|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute.FooBar|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo(Of T)
        Public ReadOnly Property Bar As T
        Public ReadOnly Property FooBar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            {method}(substitute.Bar, 1)
            {method}([|substitute.FooBar|], 1)
            {method}(value:= [|substitute.FooBar|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute.FooBar|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports NSubstitute

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
            {method}(substitute.Bar(1, 2), 1)
            {method}([|substitute.Bar(1)|], 1)
            {method}(value:= [|substitute.Bar(1)|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute.Bar(1)|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports NSubstitute

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
            {method}(substitute.Bar(Of Integer)(1, 2), 1)
            {method}([|substitute.Bar(1)|], 1)
            {method}(value:= [|substitute.Bar(1)|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute.Bar(1)|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports NSubstitute

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
            {method}(substitute(1 ,2), 1)
            {method}([|substitute(1)|], 1)
            {method}(value:= [|substitute(1)|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute(1)|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports NSubstitute

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
            {method}(substitute(1 ,2), 1)
            {method}([|substitute(1)|], 1)
            {method}(value:= [|substitute(1)|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute(1)|])
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports NSubstitute

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
            {method}(substitute(1), 1)
            {method}(value:= substitute(1), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute(1))
            {method}(substitute.Bar, 1)
            {method}(value:= substitute.Bar, returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar)
            {method}(substitute.FooBar(), 1)
            {method}(value:= substitute.FooBar(), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.FooBar())

            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()
            {method}([|substituteFooBarBar(1)|], 1)
            {method}(value:= [|substituteFooBarBar(1)|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substituteFooBarBar(1)|])
            {method}([|substituteFooBarBar.Bar|], 1)
            {method}(value:= [|substituteFooBarBar.Bar|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substituteFooBarBar.Bar|])
            {method}([|substituteFooBarBar.FooBar()|], 1)
            {method}(value:= [|substituteFooBarBar.FooBar()|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substituteFooBarBar.FooBar()|])
        End Sub
    End Class
End Namespace
";

        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports NSubstitute

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
            {method}(substitute(1), 1)
            {method}(value:= substitute(1), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute(1))
            {method}(substitute.Bar, 1)
            {method}(value:= substitute.Bar, returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar)
            {method}(substitute.FooBar(), 1)
            {method}(value:= substitute.FooBar(), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.FooBar())

            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar(Of Integer))()
            {method}([|substituteFooBarBar(1)|], 1)
            {method}(value:= [|substituteFooBarBar(1)|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substituteFooBarBar(1)|])
            {method}([|substituteFooBarBar.Bar|], 1)
            {method}(value:= [|substituteFooBarBar.Bar|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substituteFooBarBar.Bar|])
            {method}([|substituteFooBarBar.FooBar()|], 1)
            {method}(value:= [|substituteFooBarBar.FooBar()|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substituteFooBarBar.FooBar()|])
        End Sub
    End Class
End Namespace
";

        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports NSubstitute
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
            {method}(substitute(1), 1)
            {method}(value:= substitute(1), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute(1))
            {method}(substitute.Bar, 1)
            {method}(value:= substitute.Bar, returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.Bar)
            {method}(substitute.FooBar(), 1)
            {method}(value:= substitute.FooBar(), returnThis:= 1)
            {method}(returnThis:= 1, value:= substitute.FooBar())

            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()
            {method}([|substituteFooBarBar(1)|], 1)
            {method}(value:= [|substituteFooBarBar(1)|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substituteFooBarBar(1)|])
            {method}([|substituteFooBarBar.Bar|], 1)
            {method}(value:= [|substituteFooBarBar.Bar|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substituteFooBarBar.Bar|])
            {method}([|substituteFooBarBar.FooBar()|], 1)
            {method}(value:= [|substituteFooBarBar.FooBar()|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substituteFooBarBar.FooBar()|])
        End Sub
    End Class
End Namespace
";

        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted."
        }.Repeat(2).ToList();

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(MyNamespace.IFoo)~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);

        var source = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Bar = NSubstitute.Substitute.[For](Of IBar)()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            {method}(substitute.GetBar(), 1)
            {method}([|substitute.GetFooBar()|], 1)
            {method}(value:= [|substitute.GetFooBar()|], returnThis:= 1)
            {method}(returnThis:= 1, value:= [|substitute.GetFooBar()|])
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

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
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
            Dim x = {method}([|substitute{call}|], 1)
            Dim y = {method}(value:= [|substitute{call}|], returnThis:= 1)
            Dim z = {method}(returnThis:= 1, value:= [|substitute{call}|])
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
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
            Dim x = {method}(substitute{call}, 1)
            Dim y = {method}(value:= substitute{call}, returnThis:= 1)
            Dim z = {method}(returnThis:= 1, value:= substitute{call})
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
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
            Dim x = {method}([|substitute{call}|], 1)
            Dim y = {method}(value:= [|substitute{call}|], returnThis:= 1)
            Dim z = {method}(returnThis:= 1, value:= [|substitute{call}|])
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call)
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
            Dim x = {method}(substitute{call}, 1)
            Dim y = {method}(value:= substitute{call}, returnThis:= 1)
            Dim z = {method}(returnThis:= 1, value:= substitute{call})
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}