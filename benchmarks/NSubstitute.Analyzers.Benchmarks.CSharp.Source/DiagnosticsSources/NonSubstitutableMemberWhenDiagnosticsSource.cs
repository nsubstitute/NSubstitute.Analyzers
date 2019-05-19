using System.Threading.Tasks;
using NSubstitute.Analyzers.Benchmarks.CSharp.Source.Models;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources
{
    public class NonSubstitutableMemberWhenDiagnosticsSource
    {
        public void NS1002_NonVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.When(sub => sub.ObjectReturningMethod());
            substitute.When(sub =>
            {
                _ = sub.Property;
            });
            substitute.When(sub =>
            {
                _ = sub[0];
            });
            substitute.When(WhenDelegate);
            
            SubstituteExtensions.When(substitute, sub => sub.ObjectReturningMethod());
            SubstituteExtensions.When(substitute, sub =>
            {
                _ = sub.Property;
            });
            SubstituteExtensions.When(substitute, sub =>
            {
                _ = sub[0];
            });
            
            SubstituteExtensions.When(substitute, WhenDelegate);
        }
       
        public void NS1003_InternalVirtualSetupSpecification()
        {
            var substitute = Substitute.For<Foo>();

            substitute.When(sub => sub.InternalObjectReturningMethod());
            substitute.When(sub =>
            {
                _ = sub.InternalObjectReturningProperty;
            });
            substitute.When(sub =>
            {
                _ = sub[0, 0, 0];
            });
            substitute.When(WhenDelegateWithInternal);
            
            SubstituteExtensions.When(substitute, sub => sub.InternalObjectReturningMethod());
            SubstituteExtensions.When(substitute, sub =>
            {
                _ = sub.InternalObjectReturningProperty;
            });
            SubstituteExtensions.When(substitute, sub =>
            {
                _ = sub[0, 0, 0];
            });
            SubstituteExtensions.When(substitute, WhenDelegateWithInternal);
        }
        
        public Task WhenDelegate(Foo foo)
        {
            foo.ObjectReturningMethod();
            return Task.CompletedTask;
        }
        
        public Task WhenDelegateWithInternal(Foo foo)
        {
            foo.InternalObjectReturningMethod();
            return Task.CompletedTask;
        }
    }
}