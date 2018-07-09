using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReEntrantReturnsSetupAnalyzerTests
{
    public class ReturnsAsOrdinaryMethodWithGenericTypeSpecifiedTests : ReEntrantReturnsSetupDiagnosticVerifier
    {
        [Theory]
        [InlineData("substitute.Foo().Returns(1)")]
        [InlineData("substitute.Foo().Returns(1) \n\rOtherReturn()")]
        [InlineData("SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        public async Task ReturnsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall)
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
            SubstituteExtensions.Returns(Of Integer)(substitute.Bar(), ReturnThis(), OtherReturn())
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

            var firstArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(Function(x) ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 72)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(Function(x) OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 86)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        [Theory]
        [InlineData("substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("OtherReturn()\r\n substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        public async Task ReturnsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall)
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
            SubstituteExtensions.Returns(Of Integer)(substitute.Bar(), ReturnThis(), OtherReturn())
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

            var firstArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(Function(x) ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 72)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(Function(x) OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 86)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        [Fact]
        public async Task ReturnsDiagnostic_ForNestedReEntrantCall()
        {
            var source = @"Imports NSubstitute

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
            SubstituteExtensions.Returns(Of Integer)(substitute.Bar(), ReturnThis(), OtherReturn())
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), NestedReturnThis())
            Return 1
        End Function

        Private Function NestedReturnThis() As Integer
            Return OtherNestedReturnThis()
        End Function

        Private Function OtherNestedReturnThis() As Integer
            Dim [sub] = Substitute.[For](Of IBar)()
            SubstituteExtensions.Returns(Of Integer)([sub].Foo(), 1)
            Return 1
        End Function
    End Class
End Namespace
";

            var firstArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(Function(x) ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 72)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(Function(x) OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 86)
                }
            };

            var nestedArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(Function(x) NestedReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(24, 72)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic, nestedArgumentDiagnostic);
        }

        [Theory]
        [InlineData("ReturnThis()", "OtherReturn()")]
        [InlineData("ReturnThis", "OtherReturn")]
        [InlineData("1", "2")]
        [InlineData("Function(x) 1", "Function(x) 2")]
        public async Task ReturnsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn)
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
            SubstituteExtensions.Returns(Of Integer)(substitute.Bar(), {firstReturn}, {secondReturn})
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
            await VerifyDiagnostic(source);
        }
    }
}