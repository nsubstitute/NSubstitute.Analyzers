using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace NSubstitute.Analyzers.Test.CSharp.AnalyzerTests.SubstituteAnalyzersTests
{
    public abstract class SubstituteAnalyzerTests : AnalyzerTest
    {
        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount();

        // even though parameters are optional, NSubstitute requires to pass all of them
        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied();

        [Fact]
        public abstract Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2();

        [Fact]
        public abstract Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenCorrespondingConstructorArgumentsNotCompatible();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenAssigningDoubleToInt();

        // even though it is valid in c# NSubstitute will throw exception
        [Fact]
        public abstract Task ReturnsDiagnostic_WhenAssigningIntToDouble();

        [Fact]
        public abstract Task ReturnsNoDiagnostic_WhenCorrespondingConstructorArgumentsAreImplicitlyConvertible();

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }
    }
}