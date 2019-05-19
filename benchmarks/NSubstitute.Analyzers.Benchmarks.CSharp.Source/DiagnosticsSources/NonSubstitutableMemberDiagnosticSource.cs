using NSubstitute.Analyzers.Benchmarks.CSharp.Source.Models;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources
{
    public class NonSubstitutableMemberDiagnosticSource
    {
        public void NS1000_NonVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.ObjectReturningMethod().Returns(1);
            substitute.Property.Returns(1);
            substitute[0].Returns(1);
            1.Returns(1);
            
            SubstituteExtensions.Returns(substitute.ObjectReturningMethod(), 1);
            SubstituteExtensions.Returns(substitute.Property, 1);
            SubstituteExtensions.Returns(substitute[0], 1);
            SubstituteExtensions.Returns(1, 1);
        }

        public void NS1003_InternalVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.InternalObjectReturningMethod().Returns(1);
            substitute.InternalObjectReturningProperty.Returns(1);
            substitute[0, 0, 0].Returns(1);
            
            SubstituteExtensions.Returns(substitute.InternalObjectReturningMethod(), 1);
            SubstituteExtensions.Returns(substitute.InternalObjectReturningProperty, 1);
            SubstituteExtensions.Returns(substitute[0, 0, 0], 1);
        }
    }
}