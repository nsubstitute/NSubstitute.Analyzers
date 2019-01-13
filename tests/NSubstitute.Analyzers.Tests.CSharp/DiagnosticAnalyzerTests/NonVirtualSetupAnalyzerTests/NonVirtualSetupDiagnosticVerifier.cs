using System.Threading.Tasks;
using Markdig.Syntax.Inlines;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public abstract class NonVirtualSetupDiagnosticVerifier : CSharpDiagnosticVerifier, INonVirtualSetupDiagnosticVerifier
    {
        internal AnalyzersSettings Settings { get; set; }

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method);

        [CombinatoryTheory]
        [InlineData("1", "int")]
        [InlineData("'c'", "char")]
        [InlineData("true", "bool")]
        [InlineData("false", "bool")]
        [InlineData(@"""1""", "string")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForLiteral(string method, string literal, string type);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForStaticMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod(string method);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }

        protected override string GetSettings()
        {
            return Settings != null ? Json.Encode(Settings) : null;
        }
    }
}