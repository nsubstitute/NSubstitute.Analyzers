using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
{
    public class AsyncReceivedInOrderCallbackDiagnosticSource
    {
        public void NS5002_AsyncCallbackUsedInReceivedInOrderMethod()
        {
            Received.InOrder(async () => { await Task.CompletedTask; });
        }
    }
}