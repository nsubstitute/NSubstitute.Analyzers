using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace NSubstitute.Analyzers.Test.CSharp.AnalyzerTests.UnusedReceivedAnalyzerTests
{
    public class UnusedReceivedAnalyzerTests : AnalyzerTest
    {
        [Fact]
        public async Task ReportDiagnostics_WhenReceivedUsedWithoutMemberCall()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.Received(1);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Unused received check.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 13)
                }
            };


            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new UnusedReceivedAnalyzer();
        }
    }
}