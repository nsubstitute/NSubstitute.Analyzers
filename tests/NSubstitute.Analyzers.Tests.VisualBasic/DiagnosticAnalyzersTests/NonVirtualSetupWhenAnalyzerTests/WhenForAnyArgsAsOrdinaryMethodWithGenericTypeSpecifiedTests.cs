using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonVirtualSetupWhenAnalyzerTests
{
    public class WhenForAnyArgsAsOrdinaryMethodWithGenericTypeSpecifiedTests : NonVirtualSetupWhenDiagnosticVerifier
    {
        [Theory]
        [InlineData("Sub(sb) sb.Bar()", 14, 76)]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()", 14, 97)]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub",
            15,
            17)]
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string whenAction, int expectedLine, int expectedColumn)
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
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo)(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(expectedLine, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string whenAction)
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
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo)(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string whenAction)
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
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo2)(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("Sub(sb) sb()")]
        [InlineData(@"Function(ByVal [sub] As Func(Of Integer)) [sub]()")]
        [InlineData(
            @"Sub(sb As Func(Of Integer))
                sb()
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string whenAction)
        {
            var source = $@"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            SubstituteExtensions.WhenForAnyArgs(Of Func(Of Integer))(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("Sub(sb) sb.Bar()", 22, 77)]
        [InlineData(@"Function(ByVal [sub] As Foo2) [sub].Bar()", 22, 99)]
        [InlineData(
            @"Sub(sb As Foo2)
                sb.Bar()
            End Sub",
            23,
            17)]
        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string whenAction, int expectedLine, int expectedColumn)
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
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo2)(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(expectedLine, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar() As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo)(substitute,{whenAction}).[Do](Sub(callInfo) i = i +1)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As IFoo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As IFoo)
                sb.Bar()
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.WhenForAnyArgs(Of IFoo)(substitute,{whenAction}).[Do](Sub(callInfo) i = i +1)
        End Sub
    End Class
End Namespace";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData(
            @"Sub(sb As IFoo)
                Dim x = sb.Bar
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        ReadOnly Property Bar As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.WhenForAnyArgs(Of IFoo)(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("Sub(sb) sb.Bar(Of Integer)()")]
        [InlineData(@"Function(ByVal [sub] As IFoo(Of Integer)) [sub].Bar(Of Integer)()")]
        [InlineData(
            @"Sub(sb As IFoo(Of Integer))
                sb.Bar(Of Integer)()
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Interface IFoo(Of T)

        Function Bar(Of T)() As Integer
    End Interface

    Public Class FooTests

    Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo (Of Integer))()
            SubstituteExtensions.WhenForAnyArgs(Of IFoo (Of Integer))(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride ReadOnly Property Bar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo)(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData(
            @"Sub(sb As IFoo)
                Dim x = sb(1)
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal i As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.WhenForAnyArgs(Of IFoo)(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string whenAction)
        {
            var source = $@"Imports System.Runtime.CompilerServices

Namespace NSubstitute
    Public Class Foo
        Public Function Bar() As Integer
            Return 1
        End Function
    End Class

    Module SubstituteExtensions
        <Extension()>
        Function [WhenForAnyArgs](Of T)(ByVal substitute As T, ByVal substituteCall As System.Action(Of T), ByVal x As Integer) As T
            Return Nothing
        End Function
    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute As Foo = Nothing
            SubstituteExtensions.[WhenForAnyArgs](Of Foo)(substitute, {whenAction}, 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub",
            13,
            25)]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x as Integer
                x = sb.Bar
            End Sub",
            14,
            21)]
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string whenAction, int expectedLine, int expectedColumn)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public ReadOnly Property Bar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo)(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(expectedLine, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x as Integer
                x = sb.Bar
            End Sub")]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string whenAction)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public Overridable Property Bar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo)(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb(1)
            End Sub",
            19,
            25)]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x as Integer
                x = sb(1)
            End Sub",
            20,
            21)]
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string whenAction, int expectedLine, int expectedColumn)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
     Public Class Foo

        Public Default ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Throw New System.NotImplementedException
            End Get
        End Property
    End Class


    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 1
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo)(substitute,{whenAction}).[Do](Sub(callInfo) i = i + 1)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(expectedLine, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo)(substitute,AddressOf SubstituteCall).[Do](Sub(callInfo) i = i + 1)
            substitute.Bar()
        End Sub

        Private Sub SubstituteCall(ByVal [sub] As Foo)
            Dim objBarr = [sub].Bar()
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(19, 27)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim i As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.WhenForAnyArgs(Of Foo)(substitute,AddressOf SubstituteCall).[Do](Sub(callInfo) i = i + 1)
            substitute.Bar()
        End Sub

        Private Sub SubstituteCall(ByVal [sub] As Foo)
            Dim objBarr = [sub].Bar()
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        [Fact(Skip = "Expression bodied functions not supported in VB")]
        public override Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularExpressionBodiedFunction()
        {
            return Task.CompletedTask;
        }

        [Fact(Skip = "Local functions not supported in VB")]
        public override Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InLocalFunction()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public override Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InExpressionBodiedLocalFunction()
        {
            return Task.CompletedTask;
        }

        [Fact(Skip = "Expression bodied functions not supported in VB")]
        public override Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularExpressionBodiedFunction()
        {
            return Task.CompletedTask;
        }

        [Fact(Skip = "Local functions not supported in VB")]
        public override Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InLocalFunction()
        {
            return Task.CompletedTask;
        }

        [Fact(Skip = "Expression bodied local functions not supported in VB")]
        public override Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InExpressionBodiedLocalFunction()
        {
            return Task.CompletedTask;
        }
    }
}