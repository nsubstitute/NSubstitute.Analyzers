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
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod();

        [Theory]
        [InlineData("1", "int")]
        [InlineData("'c'", "char")]
        [InlineData("true", "bool")]
        [InlineData("false", "bool")]
        [InlineData(@"""1""", "string")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForStaticMethod();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod();

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