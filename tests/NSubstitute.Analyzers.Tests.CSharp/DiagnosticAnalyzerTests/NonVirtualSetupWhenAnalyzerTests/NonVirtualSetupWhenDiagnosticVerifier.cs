using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupWhenAnalyzerTests
{
    public abstract class NonVirtualSetupWhenDiagnosticVerifier : CSharpDiagnosticVerifier, INonVirtualSetupWhenDiagnosticVerifier
    {
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string whenAction, int expectedColumn);

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string whenAction);

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string whenAction);

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string whenAction);

        public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string whenAction, int expectedColumn);

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string whenAction);

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string whenAction);

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string whenAction);

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string whenAction);

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string whenAction);

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string whenAction);

        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string whenAction);

        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string whenAction, int expectedColumn);

        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string whenAction);

        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string whenAction, int expectedColumn);

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