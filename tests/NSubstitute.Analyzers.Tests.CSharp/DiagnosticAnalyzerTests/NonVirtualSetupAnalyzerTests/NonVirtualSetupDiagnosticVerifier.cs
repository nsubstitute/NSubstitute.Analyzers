using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Newtonsoft.Json;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public abstract class NonVirtualSetupDiagnosticVerifier : CSharpDiagnosticVerifier, INonVirtualSetupDiagnosticVerifier
    {
        internal AnalyzersSettings Settings { get; set; }

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod();

        [Theory]
        [InlineData("1", "int")]
        [InlineData("'c'", "char")]
        [InlineData("true", "bool")]
        [InlineData("false", "bool")]
        [InlineData(@"""1""", "string")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type);

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForStaticMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod();

        [Fact]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty();

        [Fact]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod();

        [Fact]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer();

        [Fact]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType();

        [Fact]
        public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace();

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }

        protected override string GetSettings()
        {
            return Settings != null ? JsonConvert.SerializeObject(Settings) : null;
        }
    }
}