using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ConflictingArgumentAssignmentsAnalyzerTests
{
    [CombinatoryData("AndDoes")]
    public class AndDoesAsOrdinaryMethodTests : ConflictingArgumentAssignmentsDiagnosticVerifier
    {
        public override async Task ReportsDiagnostic_When_AndDoesMethod_SetsSameArgument_AsPreviousSetupMethod(string method, string call, string previousCallArgAccess, string andDoesArgAccess)
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
            {call}.Returns(1).{method}(Function(callInfo)
               {previousCallArgAccess}
            End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source, Descriptor);
        }

        public override async Task ReportsNoDiagnostics_When_AndDoesMethod_SetsDifferentArgument_AsPreviousSetupMethod(string method, string call, string andDoesArgAccess)
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
            {call}.Returns(1).{method}(Function(callInfo)
                callInfo(0) = 1
            End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_When_AndDoesMethod_AccessSameArguments_AsPreviousSetupMethod(string method, string call, string argAccess)
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
            {call}.Returns(1).{method}(Function(callInfo)
                {argAccess}
            End Function).AndDoes(Function(callInfo)
                {argAccess}
            End Function)
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_When_AndDoesMethod_SetSameArguments_AsPreviousSetupMethod_SetsIndirectly(string method)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Integer)()).Returns(1).{method}(Function(callInfo)
                callInfo.Args()(0) = 1
                callInfo.ArgTypes()(0) = GetType(Integer)
                Dim x = (DirectCast(callInfo(0), Byte()))
                x(0) = 1
                Return 1
            End Function).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function)
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_When_AndDoesMethod_SetArgument_AndPreviousMethod_IsNotUsedWithCallInfo(string method, string call, string andDoesArgAccess)
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
            {call}.Returns(1).{method}(Function(callInfo)
                End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }
    }
}