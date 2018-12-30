using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonVirtualSetupWhenAnalyzerTests
{
    public abstract class NonVirtualSetupWhenDiagnosticVerifier : VisualBasicDiagnosticVerifier, INonVirtualSetupWhenDiagnosticVerifier
    {
        [Theory]
        [InlineData("Sub(sb) sb.Bar()", 14, 39)]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()", 14, 60)]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub",
            15,
            17)]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string whenAction, int expectedLine, int expectedColumn);

        [Theory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string whenAction);

        [Theory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string whenAction);

        [Theory]
        [InlineData("Sub(sb) sb()")]
        [InlineData(@"Function(ByVal [sub] As Func(Of Integer)) [sub]()")]
        [InlineData(
            @"Sub(sb As Func(Of Integer))
                sb()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string whenAction);

        [Theory]
        [InlineData("Sub(sb) sb.Bar()", 22, 39)]
        [InlineData(@"Function(ByVal [sub] As Foo2) [sub].Bar()", 22, 61)]
        [InlineData(
            @"Sub(sb As Foo2)
                sb.Bar()
            End Sub",
            23,
            17)]
        public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string whenAction, int expectedLine, int expectedColumn);

        [Theory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string whenAction);

        [Theory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As IFoo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As IFoo)
                sb.Bar()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string whenAction);

        [Theory]
        [InlineData(
            @"Sub(sb As IFoo)
                Dim x = sb.Bar
            End Sub")]
        [InlineData(
            @"Sub(sb As IFoo)
                Dim x as Integer
                x = sb.Bar
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string whenAction);

        [Theory]
        [InlineData("Sub(sb) sb.Bar(Of Integer)()")]
        [InlineData(@"Function(ByVal [sub] As IFoo(Of Integer)) [sub].Bar(Of Integer)()")]
        [InlineData(
            @"Sub(sb As IFoo(Of Integer))
                sb.Bar(Of Integer)()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string whenAction);

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
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string whenAction);

        [Theory]
        [InlineData(
            @"Sub(sb As IFoo)
                Dim x = sb(1)
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string whenAction);

        [Theory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string whenAction);

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
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string whenAction, int expectedLine, int expectedColumn);

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
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string whenAction);

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
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string whenAction, int expectedLine, int expectedColumn);

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction();

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupWhenAnalyzer();
        }
    }
}