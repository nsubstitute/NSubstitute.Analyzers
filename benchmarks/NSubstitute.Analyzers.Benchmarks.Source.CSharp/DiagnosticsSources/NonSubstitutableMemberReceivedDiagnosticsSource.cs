using NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
{
    public class NonSubstitutableMemberReceivedDiagnosticsSource
    {
        public void NS1001_NonVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.Received(1).ObjectReturningMethod();
            _ = substitute.Received(1).IntReturningProperty;
            _ = substitute.Received(1)[0];

            SubstituteExtensions.Received(substitute, 1).ObjectReturningMethod();
            _ = SubstituteExtensions.Received(substitute, 1).IntReturningProperty;
            _ = SubstituteExtensions.Received(substitute, 1)[0];
        }

        public void NS1003_InternalVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.Received(1).InternalObjectReturningMethod();
            _ = substitute.Received(1).InternalObjectReturningProperty;
            _ = substitute.Received(1)[0, 0, 0];

            SubstituteExtensions.Received(substitute, 1).InternalObjectReturningMethod();
            _ = SubstituteExtensions.Received(substitute, 1).InternalObjectReturningProperty;
            _ = SubstituteExtensions.Received(substitute, 1)[0, 0, 0];
        }
    }
}