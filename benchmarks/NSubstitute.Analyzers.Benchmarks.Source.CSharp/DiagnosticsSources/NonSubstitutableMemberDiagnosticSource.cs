using NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
{
    public class NonSubstitutableMemberDiagnosticSource
    {
        public void NS1000_NonVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.ObjectReturningMethod().Returns((IFoo)null);
            substitute.IntReturningProperty.Returns(1);
            substitute[0].Returns(1);
            1.Returns(1);

            SubstituteExtensions.Returns(substitute.ObjectReturningMethod(), (IFoo)null);
            SubstituteExtensions.Returns(substitute.IntReturningProperty, 1);
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