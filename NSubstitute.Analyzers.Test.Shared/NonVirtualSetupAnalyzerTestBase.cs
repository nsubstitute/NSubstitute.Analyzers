using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace NSubstitute.Analyzers.Test
{
    public abstract class NonVirtualSetupAnalyzerTestBase : AnalyzerTest
    {
        [Fact]
        public abstract Task AnalyzerReturnsDiagnostic_WhenSettingValueForNonVirtualMethod();

        [Theory]
        [InlineData("1", "int")]
        [InlineData("'c'", "char")]
        [InlineData("true", "bool")]
        [InlineData("false", "bool")]
        [InlineData(@"""1""", "string")]
        public abstract Task AnalyzerReturnsDiagnostic_WhenSettingValueForLiteral(string literal, string type);

        [Fact]
        public abstract Task AnalyzerReturnsDiagnostic_WhenSettingValueForStaticMethod();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForVirtualMethod();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod();

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
        /// <returns></returns>
        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenDataFlowAnalysisIsRequired();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForDelegate();

        [Fact]
        public abstract Task AnalyzerReturnsDiagnostics_WhenSettingValueForSealedOverrideMethod();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForAbstractMethod();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForInterfaceMethod();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForInterfaceProperty();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostic_WhenSettingValueForAbstractProperty();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForInterfaceIndexer();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostic_WhenSettingValueForVirtualProperty();

        [Fact]
        public abstract Task AnalyzerReturnsDiagnostic_WhenSettingValueForNonVirtualProperty();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForVirtualIndexer();

        [Fact]
        public abstract Task AnalyzerReturnsDiagnostics_WhenSettingValueForNonVirtualIndexer();
    }
}