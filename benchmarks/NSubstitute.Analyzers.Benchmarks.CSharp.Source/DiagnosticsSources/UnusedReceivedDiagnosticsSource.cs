using NSubstitute.Analyzers.Benchmarks.CSharp.Source.Models;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources
{
    public class UnusedReceivedDiagnosticsSource
    {
        public void NS5000_ReceivedCheck()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.Received(1);
            SubstituteExtensions.Received(substitute, 1);
        }
    }
}