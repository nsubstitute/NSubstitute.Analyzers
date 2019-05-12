using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source
{
    public class NonSubstitutableMemberWhenSource
    {
        public void Test()
        {
            var substitute = Substitute.For<Foo>();

            // invalid usages
            substitute.When(x => x.VoidReturningMethod());
            substitute.When(x => x.VoidReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>()));
            substitute.When(x => { _ = x.Property; });
            substitute.When(x => { _ = x[Arg.Any<int>()]; });
            
            SubstituteExtensions.When(substitute, x => x.VoidReturningMethod());
            SubstituteExtensions.When(substitute, x => x.VoidReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>()));
            SubstituteExtensions.When(substitute, x => { _ = x.Property; });
            SubstituteExtensions.When(substitute, x => { _ = x[Arg.Any<int>()]; });
        }
    }

    public interface IFoo
    {
        void VoidReturningMethod();

        void VoidReturningMethodWithArguments(int x, int y, decimal z);
        
        int Property { get; }
        
        int this[int i] { get; }
    }

    public abstract class AbstractFoo
    {
        public abstract void VoidReturningMethod();

        public abstract void VoidReturningMethodWithArguments(int x, int y, decimal z);
        
        public abstract int this[int i] { get; }
        
        int Property { get; }
    }

    public abstract class InheritedAbstractFoo : AbstractFoo
    {
    }

    public class Foo : IFoo
    {
        public void VoidReturningMethod()
        {
        }

        public virtual void VoidReturningVirtualMethod()
        {
        }
        
        public void VoidReturningMethodWithArguments(int x, int y, decimal z)
        {
        }
        
        public int Property { get; }
        
        public int this[int i] => 1;
    }
}