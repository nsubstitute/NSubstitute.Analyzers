using NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
{
    public class NonSubstitutableMemberReceivedInOrderDiagnosticSource
    {
        public void NS1005_NonVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            Received.InOrder(() =>
            {
                substitute.ObjectReturningMethod();
                var methodItem = substitute.ObjectReturningMethod();
                substitute.ObjectReturningMethodWithArguments(substitute.IntReturningProperty, 1, 1m);
                _ = substitute.IntReturningProperty;
                var propertyItem = substitute.IntReturningProperty;
                _ = substitute[0];
                var indexerItem = substitute[0];
                var otherIndexerItem = substitute[0];
                var usedIndexerItem = otherIndexerItem;
            });
        }

        public void NS1003_InternalVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.InternalObjectReturningMethod();
            var methodItem = substitute.InternalObjectReturningMethod();
            substitute.InternalObjectReturningMethodWithArguments(substitute.IntReturningProperty);
            _ = substitute.InternalObjectReturningProperty;
            var propertyItem = substitute.InternalObjectReturningProperty;
            _ = substitute[0];
            var indexerItem = substitute[0, 0, 0];
            var otherIndexerItem = substitute[0, 0, 0];
            var usedIndexerItem = otherIndexerItem;
        }
    }
}