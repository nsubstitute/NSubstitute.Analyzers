using NSubstitute.Analyzers.Benchmarks.Shared.Models;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources
{
    public class NonSubstitutableMemberDiagnosticSource
    {
        public void NS1000_NonVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.ObjectReturningMethod().Returns((IFoo)null);
            substitute.Property.Returns(1);
            substitute[0].Returns(1);
            1.Returns(1);

            SubstituteExtensions.Returns(substitute.ObjectReturningMethod(), (IFoo)null);
            SubstituteExtensions.Returns(substitute.Property, 1);
            SubstituteExtensions.Returns(substitute[0], 1);
            SubstituteExtensions.Returns(1, 1);
        }

        public void NS1003_InternalVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.InternalObjectReturningMethod().Returns((IFoo)null);
            substitute.InternalObjectReturningProperty.Returns((IFoo)null);
            substitute[0, 0, 0].Returns((IFoo)null);

            SubstituteExtensions.Returns(substitute.InternalObjectReturningMethod(), (IFoo)null);
            SubstituteExtensions.Returns(substitute.InternalObjectReturningProperty, (IFoo)null);
            SubstituteExtensions.Returns(substitute[0, 0, 0], (IFoo)null);
        }
    }
}