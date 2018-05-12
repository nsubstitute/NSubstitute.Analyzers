using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Test.AnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public abstract class NonVirtualSetupAnalyzerTest : AnalyzerTest<NonVirtualSetupAnalyzer>
    {
        [Fact]
        public abstract Task AnalyzerReturnsDiagnostic_WhenSettingValueForNonVirtualMethod();

        [Fact]
        public abstract Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForVirtualMethod();

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