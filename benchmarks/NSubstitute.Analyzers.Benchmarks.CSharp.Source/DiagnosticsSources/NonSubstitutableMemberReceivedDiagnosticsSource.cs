using NSubstitute.Analyzers.Benchmarks.CSharp.Source.Models;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources
{
    public class NonSubstitutableMemberReceivedDiagnosticsSource
    {
        public void NS1001_NonVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.Received(1).ObjectReturningMethod();
            _ = substitute.Received(1).Property;
            _ = substitute.Received(1)[0];

            SubstituteExtensions.Received(substitute, 1).ObjectReturningMethod();
            _ = SubstituteExtensions.Received(substitute, 1).Property;
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