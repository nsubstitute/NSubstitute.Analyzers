using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonSubstitutableMemberAnalyzerTests
{
    [CombinatoryData("ReturnsNull", "ReturnsNullForAnyArgs")]
    public class ReturnsNullAsExtensionMethodTests : NonSubstitutableMemberDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo
        Public Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            [|substitute.Bar()|].{method}()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override Task ReportsDiagnostics_WhenSettingValueForLiteral(string method, string literal, string type)
        {
            return Task.CompletedTask;
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForStaticMethod(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Shared Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            [|Foo.Bar()|].{method}()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar().{method}()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public Overrides Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            substitute.Bar().{method}()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim returnValue = substitute.Bar()
            returnValue.{method}()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions
Imports System

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of FooTests))()
            substitute().{method}()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public NotOverridable Overrides Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            [|substitute.Bar()|].{method}()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride Function Bar() As Foo
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar().{method}()
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Interface IFoo

        Function Bar() As IFoo

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Interface IFoo

       Property Bar As IFoo

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar.{method}()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Interface IFoo(Of T)

        Function Bar(Of T)() As FooTests
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo(Of Integer))()
            substitute.Bar(Of Integer).{method}()
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride ReadOnly Property Bar As Foo
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            substitute.Bar.{method}()
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Interface IFoo

        Default Property Item(ByVal i As Integer) As IFoo
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of IFoo)
            substitute(1).{method}()
        End Sub
    End Class
End Namespace";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable ReadOnly Property Bar As Foo
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            substitute.Bar.{method}()
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Public ReadOnly Property Bar As Foo
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            [|substitute.Bar|].{method}()
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer(string method)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Default Property Item(ByVal x As Integer) As Foo
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
            substitute(1).{method}()
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Default ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Throw New NotImplementedException
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            [|substitute(1)|].{method}()
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
        Public Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Module ReturnsExtensions
        <Extension>
        Function ReturnsNull(Of T)(ByVal returnValue As T, ByVal returnThis As Object) As T
            Return Nothing
        End Function

        <Extension>
        Function ReturnsNullForAnyArgs(Of T)(ByVal returnValue As T, ByVal returnThis As Object) As T
            Return Nothing
        End Function
    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute As Foo = Nothing
            substitute.Bar().{method}(Nothing)
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
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Public ReadOnly Property Bar As Foo
        Public ReadOnly Property FooBar As Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar.{method}()
            [|substitute.FooBar|].{method}()
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
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Public ReadOnly Property Bar As T
        Public ReadOnly Property FooBar As FooTests
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of FooTests))()
            substitute.Bar.{method}()
            [|substitute.FooBar|].{method}()
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
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Foo
            Return Nothing
        End Function

        Public Function Bar(ByVal x As Integer, ByVal y As Integer) As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(1, 2).{method}()
            [|substitute.Bar(1)|].{method}()
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
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Foo
            Return Nothing
        End Function

        Public Function Bar(Of T)(ByVal x As T, ByVal y As T) As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Of Integer)(1, 2).{method}()
            [|substitute.Bar(1)|].{method}()
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
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Default Public ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute(1, 2).{method}()
            [|substitute(1)|].{method}()
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
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Default Public ReadOnly Property Item(ByVal x As T) As FooTests
            Get
                Return Nothing
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As T, ByVal y As T) As FooTests
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            substitute(1, 2).{method}()
            [|substitute(1)|].{method}()
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
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As Foo

        Default Public ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooBarBar
        Public Property Bar As Foo

        Default Public ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute(1).{method}()
            substitute.Bar.{method}()
            substitute.FooBar().{method}()
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()
            [|substituteFooBarBar(1)|].{method}()
            [|substituteFooBarBar.Bar|].{method}()
            [|substituteFooBarBar.FooBar()|].{method}()
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

             var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

             await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Public Property Bar As FooTests

        Default Public ReadOnly Property Item(ByVal x As Integer) As FooTests
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As FooTests
            Return Nothing
        End Function
    End Class

    Public Class FooBarBar(Of T)
        Public Property Bar As FooTests

        Default Public ReadOnly Property Item(ByVal x As Integer) As FooTests
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As FooTests
            Return Nothing
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            substitute(1).{method}()
            substitute.Bar.{method}()
            substitute.FooBar().{method}()
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar(Of Integer))()
            [|substituteFooBarBar(1)|].{method}()
            [|substituteFooBarBar.Bar|].{method}()
            [|substituteFooBarBar.FooBar()|].{method}()
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

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions
Imports MyOtherNamespace

Namespace MyOtherNamespace
    Public Class FooBarBar
        Public Property Bar As FooBarBar

        Default Public ReadOnly Property Item(ByVal x As Integer) As FooBarBar
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As FooBarBar
            Return Nothing
        End Function
    End Class
End Namespace

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As FooBarBar

        Default Public ReadOnly Property Item(ByVal x As Integer) As FooBarBar
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As FooBarBar
            Return Nothing
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute(1).{method}()
            substitute.Bar.{method}()
            substitute.FooBar().{method}()
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()
            [|substituteFooBarBar(1)|].{method}()
            [|substituteFooBarBar.Bar|].{method}()
            [|substituteFooBarBar.FooBar()|].{method}()
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

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(MyNamespace.IFoo)~MyNamespace.FooTests", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = $@"Imports System.Runtime.CompilerServices
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Bar = NSubstitute.Substitute.[For](Of IBar)()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.GetBar().{method}()
            [|substitute.GetFooBar()|].{method}()
        End Sub
    End Class

    Module MyExtensions
        Public Property Bar As IBar

        <Extension()>
        Function GetBar(ByVal foo As IFoo) As FooTests
            Return Bar.Foo()
        End Function

        <Extension()>
        Function GetFooBar(ByVal foo As IFoo) As FooTests
            Return foo.Bar()
        End Function
    End Module

    Interface IBar
        Function Foo() As FooTests
    End Interface

    Interface IFoo
        Function Bar() As FooTests
    End Interface
End Namespace";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member GetFooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

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
            Dim x = [|substitute{call}|].{method}()
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

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
            Dim x = substitute{call}.{method}()
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

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
            Dim x = [|substitute{call}|].{method}()
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

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
            Dim x = substitute{call}.{method}()
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }
    }
}