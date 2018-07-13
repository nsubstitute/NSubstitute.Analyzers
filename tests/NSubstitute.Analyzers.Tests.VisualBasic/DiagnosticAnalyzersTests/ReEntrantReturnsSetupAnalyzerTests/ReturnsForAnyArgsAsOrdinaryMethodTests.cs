using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReEntrantReturnsSetupAnalyzerTests
{
    public class ReturnsForAnyArgsAsOrdinaryMethodTests : ReEntrantReturnsSetupDiagnosticVerifier
    {
        [Theory]
        [InlineData("substitute.Foo().Returns(1)")]
        [InlineData("substitute.Foo().Returns(1) \n\rOtherReturn()")]
        [InlineData("SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        public override async Task ReturnsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall)
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), ReturnThis(), OtherReturn())
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
                    "ReturnsForAnyArgs() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 70)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 84)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        [Theory]
        [InlineData("substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("OtherReturn()\r\n substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        public override async Task ReturnsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall)
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), ReturnThis(), OtherReturn())
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
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 70)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 84)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        [Theory]
        [InlineData("substitute.[When](Function(x) x.Foo()).[Do](Function(callInfo) 1)")]
        [InlineData("OtherReturn() \r\n substitute.[When](Function(x) x.Foo()).[Do](Function(callInfo) 1)")]
        public override async Task ReturnsDiagnostic_WhenUsingReEntrantWhenDo(string reEntrantCall)
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), ReturnThis(), OtherReturn())
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
                    "ReturnsForAnyArgs() is set with a method that itself calls Do. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 70)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls Do. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 84)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_ForNestedReEntrantCall()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), ReturnThis(), OtherReturn())
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), NestedReturnThis())
            Return 1
        End Function

        Private Function NestedReturnThis() As Integer
            Return OtherNestedReturnThis()
        End Function

        Private Function OtherNestedReturnThis() As Integer
            Dim [sub] = Substitute.[For](Of IBar)()
            SubstituteExtensions.ReturnsForAnyArgs([sub].Foo(), 1)
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
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 70)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 84)
                }
            };

            var nestedArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) NestedReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(24, 70)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic, nestedArgumentDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_ForSpecificNestedReEntrantCall()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), Function(x) ReturnThis())
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), NestedReturnThis())
            Return 1
        End Function

        Private Function NestedReturnThis() As Integer
            Return OtherNestedReturnThis()
        End Function

        Private Function OtherNestedReturnThis() As Integer
            Dim [sub] = Substitute.[For](Of IBar)()
            SubstituteExtensions.ReturnsForAnyArgs([sub].Foo(), 1)
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
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) NestedReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(24, 70)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic);
        }

        [Theory]
        [InlineData("MyMethod()", "substitute.Foo().Returns(1)")]
        [InlineData("MyProperty", "substitute.Foo().Returns(1)")]
        [InlineData("Function(x) ReturnThis()", "substitute.Foo().Returns(1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        public override async Task ReturnsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string rootCall, string reEntrantCall)
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
                SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), {rootCall})
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
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("MyMethod()", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("MyProperty", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("Function(x) ReturnThis()", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        public override async Task ReturnsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string rootCall, string reEntrantCall)
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
                SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), {rootCall})
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
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("ReturnThis()", "OtherReturn()")]
        [InlineData("ReturnThis", "OtherReturn")]
        [InlineData("1", "2")]
        [InlineData("Function(x) 1", "Function(x) 2")]
        public override async Task ReturnsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn)
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), {firstReturn}, {secondReturn})
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