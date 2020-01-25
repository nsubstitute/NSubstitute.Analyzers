using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReEntrantReturnsSetupAnalyzerTests
{
    [CombinatoryData("Returns", "ReturnsForAnyArgs")]
    public class ReturnsAsExtensionMethodTests : ReEntrantReturnsSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string method, string reEntrantCall)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}([|ReturnThis()|], [|OtherReturn()|])
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            {reEntrantCall}
            Return 1
        End Function
    End Class
End Namespace
";

            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                $"{method}() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) ReturnThis()).",
                $"{method}() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) OtherReturn())."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string method, string reEntrantCall)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}([|ReturnThis()|], [|OtherReturn()|])
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            {reEntrantCall}
            Return 1
        End Function
    End Class
End Namespace
";

            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                $"{method}() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) ReturnThis()).",
                $"{method}() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) OtherReturn())."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantWhenDo(string method, string reEntrantCall)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}([|ReturnThis()|], [|OtherReturn()|])
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            {reEntrantCall}
            Return 1
        End Function
    End Class
End Namespace
";
            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                $"{method}() is set with a method that itself calls Do. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) ReturnThis()).",
                $"{method}() is set with a method that itself calls Do. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) OtherReturn())."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsDiagnostic_ForNestedReEntrantCall(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}([|ReturnThis()|], [|OtherReturn()|])
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            substitute.Foo().{method}([|NestedReturnThis()|])
            Return 1
        End Function

        Private Function NestedReturnThis() As Integer
            Return OtherNestedReturnThis()
        End Function

        Private Function OtherNestedReturnThis() As Integer
            Dim [sub] = Substitute.[For](Of IBar)()
            [sub].Foo().{method}(1)
            Return 1
        End Function
    End Class
End Namespace
";

            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                $"{method}() is set with a method that itself calls {method}. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) ReturnThis()).",
                $"{method}() is set with a method that itself calls {method}. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) OtherReturn()).",
                $"{method}() is set with a method that itself calls {method}. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) NestedReturnThis())."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsDiagnostic_ForSpecificNestedReEntrantCall(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}(Function(x)  ReturnThis())
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            substitute.Foo().{method}([|NestedReturnThis()|])
            Return 1
        End Function

        Private Function NestedReturnThis() As Integer
            Return OtherNestedReturnThis()
        End Function

        Private Function OtherNestedReturnThis() As Integer
            Dim [sub] = Substitute.[For](Of IBar)()
            [sub].Foo().{method}(1)
            Return 1
        End Function
    End Class
End Namespace
";

            await VerifyDiagnostic(source, Descriptor, $"{method}() is set with a method that itself calls {method}. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) NestedReturnThis()).");
        }

        public override async Task ReportsNoDiagnostic_WhenReturnsValueIsCreated_BeforeSetup(string method, string localVariable)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
        Function Bar() As IBar
    End Interface

    Public Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            {localVariable}
            Dim substitute = NSubstitute.Substitute.[For] (Of IFoo)()
            substitute.Bar().{method}(barr)
        End Sub

        Public Function Bar() As IBar
            Dim substitute = NSubstitute.Substitute.[For] (Of IBar)()
            substitute.Foo().{method}(1)
            Return substitute
        End Function
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string method, string rootCall, string reEntrantCall)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.Core
Imports System

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}({rootCall})
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            {reEntrantCall}
            Return 1
        End Function

        Private Function ReturnThisWithCallInfo(ByVal info As CallInfo) As Integer
            Return OtherReturn()
        End Function

        Private Function MyMethod() As Func(Of CallInfo, Integer)
            Return AddressOf ReturnThisWithCallInfo
        End Function

        Private ReadOnly Property MyProperty As Func(Of CallInfo, Integer)
            Get
                Return AddressOf ReturnThisWithCallInfo
            End Get
        End Property
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string method, string rootCall, string reEntrantCall)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.Core
Imports System

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}({rootCall})
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            {reEntrantCall}
            Return 1
        End Function

        Private Function ReturnThisWithCallInfo(ByVal info As CallInfo) As Integer
            Return OtherReturn()
        End Function

        Private Function MyMethod() As Func(Of CallInfo, Integer)
            Return AddressOf ReturnThisWithCallInfo
        End Function

        Private ReadOnly Property MyProperty As Func(Of CallInfo, Integer)
            Get
                Return AddressOf ReturnThisWithCallInfo
            End Get
        End Property
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string method, string firstReturn, string secondReturn)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}({firstReturn}, {secondReturn})
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Return 1
        End Function

        Private Function ReturnThis(ByVal info As CallInfo) As Integer
            Return OtherReturn(info)
        End Function

        Private Function OtherReturn(ByVal info As CallInfo) As Integer
            Return 1
        End Function
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturns_AcrossMultipleFiles(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}([|FooBar.ReturnThis()|])
        End Sub
    End Class
End Namespace
";

            var secondSource = $@"

Imports NSubstitute

Namespace MyNamespace
    Public Class FooBar
        Public Shared Function ReturnThis() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            substitute.Foo().{method}(1)
            Return 1
        End Function
    End Class
End Namespace
";

            await VerifyDiagnostics(new[] { source, secondSource }, Descriptor, $"{method}() is set with a method that itself calls {method}. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) FooBar.ReturnThis()).");
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturns_InAsyncMethod(string method)
        {
            var source = $@"Imports System.Threading.Tasks
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Public Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Async Function Test() As Task
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}([|Await ReturnThis()|])
        End Function

        Private Async Function ReturnThis() As Task(Of Integer)
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}(1)
            Return Await Task.FromResult(1)
        End Function
    End Class
End Namespace";

            await VerifyDiagnostic(source, Descriptor, $"{method}() is set with a method that itself calls {method}. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) Await ReturnThis()).");
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturnsIn_InParamsArray(string method, string reEntrantArrayCall)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For] (Of IFoo)()
            substitute.Bar().{method}(1, {reEntrantArrayCall})
        End Sub

        Private Function ReturnThis() As Integer
            Dim substitute = NSubstitute.Substitute.[For] (Of IFoo)()
            substitute.Bar().{method}(1)
            Return 1
        End Function

        Private Function CreateDefaultValue() As Integer
            Return 1
        End Function
    End Class
End Namespace";

            await VerifyDiagnostic(source, Descriptor, $"{method}() is set with a method that itself calls {method}. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) ReturnThis()).");
        }

        public override async Task ReportsNoDiagnostic_WhenUsingReEntrantReturnsIn_AndParamArrayIsNotCreatedInline(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For] (Of IFoo)()
            Dim array = New Integer() {{ CreateDefaultValue(), ReturnThis() }}
            substitute.Bar().{method}(1, array)
        End Sub

        Private Function ReturnThis() As Integer
            Dim substitute = NSubstitute.Substitute.[For] (Of IFoo)()
            substitute.Bar().{method}(1)
            Return 1
        End Function

        Private Function CreateDefaultValue() As Integer
            Return 1
        End Function
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenUsed_WithTypeofExpression(string method, string type)
        {
            var source = $@"Imports NSubstitute
Imports System
Namespace MyNamespace
    Public Class Foo
        Public Function FooBar() As Type
            Return Nothing
        End Function

        Public Function Bar() As Type
            Dim [sub] = Substitute.[For](Of Foo)()
            [sub].FooBar().Returns(GetType(Object))
            Return Nothing
        End Function
    End Class

    Public Structure FooBar
        Public Function Bar() As Type
            Dim [sub] = Substitute.[For](Of Foo)()
            [sub].FooBar().Returns(GetType(Object))
            Return Nothing
        End Function
    End Structure

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.FooBar().{method}(GetType([{type}]))
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenReturnsValueIsSet_InForEachLoop(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Public Interface IFoo
        Function Bar() As Integer
    End Interface

    Public Class FooBar
        Public Property Value As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()

            For Each fooBar In New FooBar(-1) {{}}
                substitute.Bar().{method}(fooBar.Value)
            Next
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenElementUsedTwice_InForEachLoop(string method)
        {
            var source = $@"Imports NSubstitute
Imports System.Collections.Generic
Imports System.Linq

Namespace MyNamespace
    Public Class FooTests
        Private firstEnumerator As IEnumerator(Of Integer) = Substitute.[For](Of IEnumerator(Of Integer))()
        Private secondEnumerator As IEnumerator(Of Integer) = Substitute.[For](Of IEnumerator(Of Integer))()

        Public Sub Test()
            Dim thirdEnumerator = Substitute.[For](Of IEnumerator(Of Integer))()
            Dim fourthEnumerator = Substitute.[For](Of IEnumerator(Of Integer))()

            For Each value In Enumerable.Empty(Of Integer)()
                firstEnumerator.Current.{method}(value + 1)
                firstEnumerator.Current.{method}(value + 1)
                secondEnumerator.Current.{method}(value + 1)
                thirdEnumerator.Current.{method}(value + 1)
                fourthEnumerator.Current.{method}(value + 1)
            Next
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenReturnValueIsCalledWhileBeingConfigured(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Interface IFoo
            ReadOnly Property Id As Integer
        End Interface

        Public Sub Test()
            Dim firstSubstitute = Substitute.[For](Of IFoo)()
            firstSubstitute.Id.Returns(45)

            Dim secondSubstitute = Substitute.[For](Of IFoo)()
            secondSubstitute.Id.{method}([|firstSubstitute.Id|])
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, Descriptor, $"{method}() is set with a method that itself calls Id. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) firstSubstitute.Id).");
        }

        public override async Task ReportsDiagnostics_WhenReturnValueIsCalledWhileBeingConfiguredInConstructorBody(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Private firstSubstitute As IFoo = Substitute.[For](Of IFoo)()

        Public Sub New()
            firstSubstitute.Id.Returns(45)
        End Sub

        Public Interface IFoo
            ReadOnly Property Id As Integer
        End Interface

        Public Sub Test()
            Dim secondSubstitute = Substitute.[For](Of IFoo)()
            secondSubstitute.Id.{method}([|firstSubstitute.Id|])
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, Descriptor, $"{method}() is set with a method that itself calls Id. This can cause problems with NSubstitute. Consider replacing with a lambda: {method}(Function(x) firstSubstitute.Id).");
        }

        public override async Task ReportsNoDiagnostics_WhenReturnValueIsCalledAfterIsConfigured(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Interface IFoo
            ReadOnly Property Id As Integer
            ReadOnly Property OtherId As Integer
        End Interface

        Public Sub Test()
            Dim firstSubstitute = Substitute.[For](Of IFoo)()
            Dim secondSubstitute = Substitute.[For](Of IFoo)()
            Dim thirdSubstitute = Substitute.[For](Of IFoo)()
            Dim fourthSubstitute = Substitute.[For](Of IFoo)()
            firstSubstitute.OtherId.Returns(45)
            thirdSubstitute.Id.Returns(45)
            fourthSubstitute.Id.Returns(45)
            Dim value = fourthSubstitute.Id
            secondSubstitute.Id.{method}(firstSubstitute.Id)
            secondSubstitute.Id.{method}(value)
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegateInArrayParams_AndReEntrantReturnsForAnyArgsCallExists(string method)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.Core
Imports System

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Interface IBar
        Function Foo() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}(Function(x) 1, New Func(Of CallInfo, Integer)() {{Function(y) OtherReturn()}})
        End Sub

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            substitute.Foo().Returns(1)
            Return 1
        End Function
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }
    }
}