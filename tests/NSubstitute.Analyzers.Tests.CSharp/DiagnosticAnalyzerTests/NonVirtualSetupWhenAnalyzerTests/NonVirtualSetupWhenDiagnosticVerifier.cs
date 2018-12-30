using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupWhenAnalyzerTests
{
    public abstract class NonVirtualSetupWhenDiagnosticVerifier : CSharpDiagnosticVerifier, INonVirtualSetupWhenDiagnosticVerifier
    {
        [Theory]
        [InlineData("sub => sub.Bar()", 19, 36)]
        [InlineData("delegate(Foo sub) { sub.Bar(); }", 19, 49)]
        [InlineData("sub => { sub.Bar(); }", 19, 38)]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string whenAction, int expectedLine, int expectedColumn);

        [Theory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string whenAction);

        [Theory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo2 sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string whenAction);

        [Theory]
        [InlineData("sub => sub()")]
        [InlineData("delegate(Func<int> sub) { sub(); }")]
        [InlineData("sub => { sub(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string whenAction);

        [Theory]
        [InlineData("sub => sub.Bar()", 24, 36)]
        [InlineData("delegate(Foo2 sub) { sub.Bar(); }", 24, 50)]
        [InlineData("sub => { sub.Bar(); }", 24, 38)]
        public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string whenAction, int expectedLine, int expectedColumn);

        [Theory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string whenAction);

        [Theory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(IFoo sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string whenAction);

        [Theory]
        [InlineData("delegate(IFoo sub) { var x = sub.Bar; }")]
        [InlineData("sub => { int x; x = sub.Bar; }")]
        [InlineData("sub => { var x = sub.Bar; }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string whenAction);

        [Theory]
        [InlineData("sub => sub.Bar<int>()")]
        [InlineData("delegate(IFoo<int> sub) { sub.Bar<int>(); }")]
        [InlineData("sub => { sub.Bar<int>(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string whenAction);

        [Theory]
        [InlineData("sub => { var x = sub.Bar; }")]
        [InlineData("sub => { int x; x = sub.Bar; }")]
        [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string whenAction);

        [Theory]
        [InlineData("delegate(IFoo sub) { var x = sub[1]; }")]
        [InlineData("sub => { var x = sub[1]; }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string whenAction);

        [Theory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string whenAction);

        [Theory]
        [InlineData("sub => { var x = sub.Bar; }", 16, 46)]
        [InlineData("sub => { int x; x = sub.Bar; }", 16, 49)]
        [InlineData("delegate(Foo sub) { var x = sub.Bar; }", 16, 57)]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string whenAction, int expectedLine, int expectedColumn);

        [Theory]
        [InlineData("sub => { var x = sub.Bar; }")]
        [InlineData("sub => { int x; x = sub.Bar; }")]
        [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string whenAction);

        [Theory]
        [InlineData("sub => { var x = sub[1]; }", 16, 46)]
        [InlineData("delegate(Foo sub) { var x = sub[1]; }", 16, 57)]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string whenAction, int expectedLine, int expectedColumn);

        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InLocalFunction();

        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InExpressionBodiedLocalFunction();

        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction();

        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularExpressionBodiedFunction();

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InLocalFunction();

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InExpressionBodiedLocalFunction();

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction();

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularExpressionBodiedFunction();

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupWhenAnalyzer();
        }
    }
}