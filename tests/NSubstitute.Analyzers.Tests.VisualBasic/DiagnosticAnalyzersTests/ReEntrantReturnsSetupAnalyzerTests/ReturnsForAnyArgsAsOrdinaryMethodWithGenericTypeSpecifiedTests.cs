using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReEntrantReturnsSetupAnalyzerTests
{
    public class ReturnsForAnyArgsAsOrdinaryMethodWithGenericTypeSpecifiedTests : ReEntrantReturnsSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall)
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
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Bar(), ReturnThis(), OtherReturn())
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
                    new DiagnosticResultLocation(15, 82)
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
                    new DiagnosticResultLocation(15, 96)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall)
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
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Bar(), ReturnThis(), OtherReturn())
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
                    new DiagnosticResultLocation(15, 82)
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
                    new DiagnosticResultLocation(15, 96)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantWhenDo(string reEntrantCall)
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
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Bar(), ReturnThis(), OtherReturn())
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
                    new DiagnosticResultLocation(15, 82)
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
                    new DiagnosticResultLocation(15, 96)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        public override async Task ReportsDiagnostic_ForNestedReEntrantCall()
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
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Bar(), ReturnThis(), OtherReturn())
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), NestedReturnThis())
            Return 1
        End Function

        Private Function NestedReturnThis() As Integer
            Return OtherNestedReturnThis()
        End Function

        Private Function OtherNestedReturnThis() As Integer
            Dim [sub] = Substitute.[For](Of IBar)()
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)([sub].Foo(), 1)
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
                    new DiagnosticResultLocation(15, 82)
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
                    new DiagnosticResultLocation(15, 96)
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
                    new DiagnosticResultLocation(24, 82)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic, nestedArgumentDiagnostic);
        }

        public override async Task ReportsDiagnostic_ForSpecificNestedReEntrantCall()
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
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Bar(), Function(x) ReturnThis())
        End Sub

        Private Function ReturnThis() As Integer
            Return OtherReturn()
        End Function

        Private Function OtherReturn() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), NestedReturnThis())
            Return 1
        End Function

        Private Function NestedReturnThis() As Integer
            Return OtherNestedReturnThis()
        End Function

        Private Function OtherNestedReturnThis() As Integer
            Dim [sub] = Substitute.[For](Of IBar)()
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)([sub].Foo(), 1)
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
                    new DiagnosticResultLocation(24, 82)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic);
        }

        public override async Task ReportsNoDiagnostic_WhenReturnsValueIsCreated_BeforeSetup(string localVariable)
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
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.ReturnsForAnyArgs(Of IBar)(substitute.Bar(), barr)
        End Sub

        Public Function Bar() As IBar
            Dim substitute = NSubstitute.Substitute.[For] (Of IBar)()
            substitute.Foo().Returns(1)
            Return substitute
        End Function
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string rootCall, string reEntrantCall)
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
                SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Bar(), {rootCall})
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

        public override async Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string rootCall, string reEntrantCall)
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
                SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Bar(), {rootCall})
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

        public override async Task ReportsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn)
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
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Bar(), {firstReturn}, {secondReturn})
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

        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturns_AcrossMultipleFiles()
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
            SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Bar(), FooBar.ReturnThis())
        End Sub
    End Class
End Namespace
";

            var secondSource = @"

Imports NSubstitute

Namespace MyNamespace
    Public Class FooBar
        Public Shared Function ReturnThis() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IBar)()
            substitute.Foo().ReturnsForAnyArgs(1)
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
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(Function(x) FooBar.ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 82)
                }
            };

            await VerifyDiagnostic(new[] { source, secondSource }, firstArgumentDiagnostic);
        }
    }
}