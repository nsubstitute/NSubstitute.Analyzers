using NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
{
    public class ReceivedLikeUsedInReceivedInOrderDiagnosticSource
    {
        public void NS5001_ReceivedLikeUsedInReceivedInOrderCallback()
        {
            var substitute = Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                substitute.Received().ObjectReturningMethod();
                _ = substitute.Received().InternalObjectReturningProperty;
                _ = substitute.Received()[0];

                SubstituteExtensions.Received(substitute).ObjectReturningMethod();
                _ = SubstituteExtensions.Received(substitute).InternalObjectReturningProperty;
                _ = SubstituteExtensions.Received(substitute)[0];
            });
        }
    }
}