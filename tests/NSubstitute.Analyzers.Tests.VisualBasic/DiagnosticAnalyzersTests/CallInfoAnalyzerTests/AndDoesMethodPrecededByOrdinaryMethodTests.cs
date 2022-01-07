using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.CallInfoAnalyzerTests;

[CombinatoryData("AndDoes")]
public class AndDoesMethodPrecededByOrdinaryMethodTests : CallInfoDiagnosticVerifier
{
    public override async Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default  ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim returnedValue = SubstituteExtensions.Returns({call}, 1)
            returnedValue.{method}(Function(callInfo)
                                      {argAccess}
                                  End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                               {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoArgumentOutOfRangeDescriptor, "There is no argument at position 1");
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                                   {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal Optional y As Integer = 1) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal Optional y As Integer = 1) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                                   {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBoundsForNestedCall(string method)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Interface IFooBar
        Function FooBaz(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFooBar)()
            SubstituteExtensions.Returns(substitute.FooBaz(Arg.Any(Of Integer)(), Arg.Any(Of Integer)()), 1).{method}(Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
                otherSubstitute.Bar(Arg.Any(Of Integer)()).Returns(Function(innerCallInfo)
                    Dim x = outerCallInfo.ArgAt(Of Integer)(1)
                    Dim y = outerCallInfo(1)
                    Dim xx = innerCallInfo.ArgAt(Of Integer)(0)
                    Dim yy = innerCallInfo(0)
                    Return 1
                End Function)
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBoundsForNestedCall(string method)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Interface IFooBar
        Function FooBaz(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFooBar)()
            SubstituteExtensions.Returns(substitute.FooBaz(Arg.Any(Of Integer)(), Arg.Any(Of Integer)()), 1).{method}(Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
                otherSubstitute.Bar(Arg.Any (Of Integer)()).Returns(Function(innerCallInfo)
                    Dim x = [|outerCallInfo.ArgAt(Of Integer)(2)|]
                    Dim y = [|outerCallInfo(2)|]
                    Dim z = outerCallInfo(1)
                    Dim xx = [|innerCallInfo.ArgAt(Of Integer)(1)|]
                    Dim yy = [|innerCallInfo(1)|]
                    Dim zz = innerCallInfo(0)
                    Return 1
                End Function)
            End Function)
        End Sub
    End Class
End Namespace";
        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "There is no argument at position 2",
            "There is no argument at position 2",
            "There is no argument at position 1",
            "There is no argument at position 1"
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(CallInfoArgumentOutOfRangeDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();
        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Bar) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Bar) As Integer
    End Interface

    Public Class BarBase
    End Class

    Public Class Bar
        Inherits BarBase
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                                   {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Integer
        Function Foo(ByVal x As Integer, ByVal bar As FooBar) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal bar As FooBar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooBar
        Inherits Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                               {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, CallInfoCouldNotConvertParameterAtPositionDescriptor, "Couldn't convert parameter at position 1 to type MyNamespace.Bar.");
    }

    public override async Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Bar) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Bar) As Integer
    End Interface

    Public Class BarBase
    End Class

    Public Class Bar
        Inherits BarBase
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                                   {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string method, string call, string argAccess, string message)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Integer
        Function Foo(ByVal x As Integer, ByVal bar As FooBar) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal bar As FooBar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooBar
        Inherits Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                               {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoCouldNotConvertParameterAtPositionDescriptor, message);
    }

    public override async Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Bar) As Integer
        Default ReadOnly Property Item(ByVal x As Bar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                               {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Bar) As Integer
        Default ReadOnly Property Item(ByVal x As Bar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                               {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string method, string call, string argAccess, string message)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer

    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                               {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, CallInfoCouldNotFindArgumentToThisCallDescriptor, message);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal x As Integer) As Integer
        Function Bar(ByVal x As Foo) As Integer
        Function Bar(ByVal x As Integer, ByVal y As Object) As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
        Default ReadOnly Property Item(ByVal x As Foo) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y as Object) As Integer
    End Interface

    Public Class FooBase
    End Class

    Public Class Foo
        Inherits FooBase
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                               {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInvocationForNestedCall(string method)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Interface IFooBar
        Function FooBaz(ByVal x As String) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFooBar)()
            SubstituteExtensions.Returns(substitute.FooBaz(Arg.Any(Of String)()), 1).{method}(Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
                otherSubstitute.Bar(Arg.Any(Of Integer)()).Returns(Function(innerCallInfo)
                    Dim x = outerCallInfo.Arg(Of String)()
                    Return innerCallInfo.Arg(Of Integer)()
                End Function)
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string method, string call, string argAccess, string message)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
        Function Bar(ByVal x As Object, ByVal y As Object) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Integer
        Default ReadOnly Property Item(ByVal x As Object, ByVal y As Object) As Integer
    End Interface

    Public Class FooBar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                               {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoMoreThanOneArgumentOfTypeDescriptor, message);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string method, string call, string argAccess)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Integer
        Function Bar(ByVal x As Object, ByVal y As FooBar) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Integer
        Default ReadOnly Property Item(ByVal x As Object, ByVal y As FooBar) As Integer
    End Interface

    Public Class FooBar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                               {argAccess}
                           End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string method, string call)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns({call}, 1).{method}(Function(callInfo)
                               [|callInfo(1)|] = 1
                           End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoArgumentIsNotOutOrRefDescriptor, "Could not set argument 1 (Double) as it is not an out or ref argument.");
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument(string method)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByRef x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns(substitute.Bar(value), 1).{method}(Function(callInfo)
                                              callInfo(0) = 1
                                          End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument(string method)
    {
        var source = $@"Imports NSubstitute
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns(substitute.Bar(value), 1).{method}(Function(callInfo)
                                              callInfo(0) = 1
                                          End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument(string method)
    {
        var source = $@"Imports NSubstitute
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns(substitute.Bar(value), 1).{method}(Function(callInfo)
                                              [|callInfo(1)|] = 1
                                          End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoArgumentOutOfRangeDescriptor, "There is no argument at position 1");
    }

    public override async Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string method, string left, string right, string expectedMessage)
    {
        var source = $@"Imports NSubstitute
Imports System.Runtime.InteropServices
Imports System.Collections.Generic

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As {left}) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As {left} = Nothing
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns(substitute.Bar(value), 1).{method}(Function(callInfo)
                                              [|callInfo(0)|] = {right}
                                          End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, CallInfoArgumentSetWithIncompatibleValueDescriptor, expectedMessage);
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string method, string left, string right)
    {
        var source = $@"Imports NSubstitute
Imports System.Runtime.InteropServices
Imports System.Collections.Generic

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As {left}) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As {left} = Nothing
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns(substitute.Bar(value), 1).{method}(Function(callInfo)
                                              callInfo(0) = {right}
                                          End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocationForNestedCall(string method)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Interface IFooBar
        Function FooBaz(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFooBar)()
            SubstituteExtensions.Returns(substitute.FooBaz(Arg.Any(Of Integer)()), 1).{method}(Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
                otherSubstitute.Bar(Arg.Any(Of Integer)()).Returns(Function(innerCallInfo)
                    Dim x = [|outerCallInfo.Arg(Of String)()|]
                    Dim y = [|innerCallInfo.Arg(Of String)()|]
                    Return 1
                End Function)
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, CallInfoCouldNotFindArgumentToThisCallDescriptor, "Can not find an argument of type String to this call.");
    }
}