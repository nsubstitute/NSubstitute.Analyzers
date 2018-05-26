using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Test
{
    public abstract class NonVirtualSetupAnalyzerTestBase : AnalyzerTest
    {
        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod();

        [Theory]
#if CSHARP
        [InlineData("1", "int")]
        [InlineData("'c'", "char")]
        [InlineData("true", "bool")]
        [InlineData("false", "bool")]
        [InlineData(@"""1""", "string")]
#elif VISUAL_BASIC
        [InlineData("1", "Integer")]
        [InlineData(@"""c""C", "Char")]
        [InlineData("true", "Boolean")]
        [InlineData("false", "Boolean")]
        [InlineData(@"""1""", "String")]
#endif
        public abstract Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type);

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForStaticMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod();

        /// <summary>
        /// As for today cases where setup is done indirectly e.g
        /// <code>
        /// var substitute = NSubstitute.Substitute.For&lt;Foo&gt;();
        /// var returnValue = substitute.Bar();
        /// SubstituteExtensions.ReturnsForAnyArgs&lt;int&gt;(returnValue, 1);
        /// </code>
        /// are not correctly analyzed as they require data flow analysys,
        /// this test makes sure that such cases are ignored and does not produces a false warnings
        /// </summary>
        /// <returns>Task</returns>
        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod();
    }
}