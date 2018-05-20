using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xunit;

namespace NSubstitute.Analyzers.Test.VisualBasic.AnalyzerTests.UnusedReceivedAnalyzerTests
{
    public class DidNotReceiveAsOrdinaryMethodTests : UnusedReceivedAnalyzerTests
    {
        [Fact]
        public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.DidNotReceive(substitute)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.UnusedReceived,
                Severity = DiagnosticSeverity.Warning,
                Message = "Unused received check.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(10, 13)
                }
            };


            await VerifyVisualBasicDiagnostic(source, expectedDiagnostic);
        }


        [Fact]
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
            SubstituteExtensions.DidNotReceive(substitute).Bar()
        End Sub
    End Class
End Namespace
";

            await VerifyVisualBasicDiagnostic(source);
        }

        [Fact]
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
            Dim bar = SubstituteExtensions.DidNotReceive(substitute).Bar
        End Sub
    End Class
End Namespace
";

            await VerifyVisualBasicDiagnostic(source);
        }

        [Fact]
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
            Dim bar = SubstituteExtensions.DidNotReceive(substitute)(0)
        End Sub
    End Class
End Namespace
";

            await VerifyVisualBasicDiagnostic(source);
        }
    }
}