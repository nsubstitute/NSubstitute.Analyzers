using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonVirtualSetupWhenAnalyzerTests
{
    public abstract class NonVirtualSetupWhenDiagnosticVerifier : VisualBasicDiagnosticVerifier, INonVirtualSetupWhenDiagnosticVerifier
    {
        protected DiagnosticDescriptor NonVirtualWhenSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualWhenSetupSpecification;

        protected DiagnosticDescriptor InternalWhenSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.InternalWhenSetupSpecification;

        [CombinatoryTheory]
        [InlineData("Sub(sb) [|sb.Bar()|]")]
        [InlineData(@"Function(ByVal [sub] As Foo) [|[sub].Bar()|]")]
        [InlineData(
            @"Sub(sb As Foo)
                [|sb.Bar()|]
            End Sub")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("Sub(sb) sb()")]
        [InlineData(@"Function(ByVal [sub] As Func(Of Integer)) [sub]()")]
        [InlineData(
            @"Sub(sb As Func(Of Integer))
                sb()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("Sub(sb) [|sb.Bar()|]")]
        [InlineData(@"Function(ByVal [sub] As Foo) [|[sub].Bar()|]")]
        [InlineData(
            @"Sub(sb As Foo)
                [|sb.Bar()|]
            End Sub")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x as Integer
                x = sb.Bar
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("Sub(sb) sb.Bar(Of Integer)()")]
        [InlineData(@"Function(ByVal [sub] As Foo(Of Integer)) [sub].Bar(Of Integer)()")]
        [InlineData(
            @"Sub(sb As Foo(Of Integer))
                sb.Bar(Of Integer)()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x as Integer
                x = sb.Bar
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb(1)
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("Sub(sb) sb.Bar()")]
        [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar()")]
        [InlineData(
            @"Sub(sb As Foo)
                sb.Bar()
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = [|sb.Bar|]
            End Sub")]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x as Integer
                x = [|sb.Bar|]
            End Sub")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x as Integer
                x = sb.Bar
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = [|sb(1)|]
            End Sub")]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x as Integer
                x = [|sb(1)|]
            End Sub")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction(string method);

        [CombinatoryTheory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = [|sb.Bar|]
            End Sub", "Internal member Bar can not be intercepted.")]
        [InlineData(@"Sub(sb As Foo) [|sb.FooBar()|]", "Internal member FooBar can not be intercepted.")]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = [|sb(0)|]
            End Sub", "Internal member Item can not be intercepted.")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

        [CombinatoryTheory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
        [InlineData(@"Sub(sb As Foo) sb.FooBar()")]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb(0)
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

        [CombinatoryTheory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = [|sb.Bar|]
            End Sub", "Internal member Bar can not be intercepted.")]
        [InlineData(@"Sub(sb As Foo) [|sb.FooBar()|]", "Internal member FooBar can not be intercepted.")]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = [|sb(0)|]
            End Sub", "Internal member Item can not be intercepted.")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

        [CombinatoryTheory]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
        [InlineData(@"Sub(sb As Foo) sb.FooBar()")]
        [InlineData(
            @"Sub(sb As Foo)
                Dim x = sb(0)
            End Sub")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupWhenAnalyzer();
        }
    }
}