using NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
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