﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupWhenAnalyzerTests
{
    public abstract class NonVirtualSetupWhenDiagnosticVerifier : CSharpDiagnosticVerifier, INonVirtualSetupWhenDiagnosticVerifier
    {
        [CombinatoryTheory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo2 sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => sub()")]
        [InlineData("delegate(Func<int> sub) { sub(); }")]
        [InlineData("sub => { sub(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo2 sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(IFoo sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("delegate(IFoo sub) { var x = sub.Bar; }")]
        [InlineData("sub => { int x; x = sub.Bar; }")]
        [InlineData("sub => { var x = sub.Bar; }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => sub.Bar<int>()")]
        [InlineData("delegate(IFoo<int> sub) { sub.Bar<int>(); }")]
        [InlineData("sub => { sub.Bar<int>(); }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => { var x = sub.Bar; }")]
        [InlineData("sub => { int x; x = sub.Bar; }")]
        [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("delegate(IFoo sub) { var x = sub[1]; }")]
        [InlineData("sub => { var x = sub[1]; }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => { var x = sub.Bar; }")]
        [InlineData("sub => { int x; x = sub.Bar; }")]
        [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => { var x = sub.Bar; }")]
        [InlineData("sub => { int x; x = sub.Bar; }")]
        [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData("sub => { var x = [|sub[1]|]; }")]
        [InlineData("delegate(Foo sub) { var x = [|sub[1]|]; }")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method, string whenAction);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InLocalFunction(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InExpressionBodiedLocalFunction(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularExpressionBodiedFunction(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InLocalFunction(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InExpressionBodiedLocalFunction(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularExpressionBodiedFunction(string method);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupWhenAnalyzer();
        }
    }
}