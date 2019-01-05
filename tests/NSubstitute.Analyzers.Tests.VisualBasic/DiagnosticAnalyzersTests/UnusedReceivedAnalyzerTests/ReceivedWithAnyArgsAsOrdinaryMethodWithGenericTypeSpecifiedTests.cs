using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.UnusedReceivedAnalyzerTests
{
    public class ReceivedWithAnyArgsAsOrdinaryMethodWithGenericTypeSpecifiedTests : UnusedReceivedDiagnosticVerifier
    {
        public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.ReceivedWithAnyArgs(Of IFoo)(substitute)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.UnusedReceived,
                Severity = DiagnosticSeverity.Warning,
                Message = @"Unused received check. To fix, make sure there is a call after ""ReceivedWithAnyArgs"". Correct: ""SubstituteExtensions.ReceivedWithAnyArgs(sub).SomeCall();"". Incorrect: ""SubstituteExtensions.ReceivedWithAnyArgs(sub);""",
                Locations = new[]
                {
                    new DiagnosticResultLocation(10, 13)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class FooBar
    End Class

    Interface IFoo
        Function Bar() As FooBar
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.ReceivedWithAnyArgs(Of IFoo)(substitute).Bar()
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Property Bar As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            Dim bar = SubstituteExtensions.ReceivedWithAnyArgs(Of IFoo)(substitute).Bar
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            Dim bar = SubstituteExtensions.ReceivedWithAnyArgs(Of IFoo)(substitute)(0)
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate()
        {
            var source = @"Imports NSubstitute
Imports System

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            SubstituteExtensions.ReceivedWithAnyArgs(Of Func(Of Integer))(substitute)()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod()
        {
            var source = @"Imports System
Imports System.Runtime.CompilerServices

Namespace NSubstitute
    Module SubstituteExtensions
        <Extension()>
        Function ReceivedWithAnyArgs(Of T As Class)(ByVal substitute As T, ByVal params As Decimal) As T
            Return Nothing
        End Function
    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            SubstituteExtensions.ReceivedWithAnyArgs(Of Func(Of Integer))(substitute, 1D)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }
    }
}