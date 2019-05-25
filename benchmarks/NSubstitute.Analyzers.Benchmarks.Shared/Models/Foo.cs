using NSubstitute.Core;

namespace NSubstitute.Analyzers.Benchmarks.Shared.Models
{
    public class Foo : IFoo
    {
        public void VoidReturningMethod()
        {
        }

        public IFoo ObjectReturningMethod() => null;

        public ConfiguredCall CallInfoReturningMethod()
        {
            return null;
        }

        public ConfiguredCall ConfiguredCallReturningProperty { get; }

        public ConfiguredCall this[int a, int b, int c, int d] => null;

        public virtual void VoidReturningVirtualMethod()
        {
        }

        public void VoidReturningMethodWithArguments(int x, int y, decimal z)
        {
        }

        public IFoo ObjectReturningMethodWithArguments(int x, int y, decimal z) => null;

        public IFoo ObjectReturningMethodWithRefArguments(ref int x, ref int y, ref decimal z) => null;

        public int Property { get; }

        public IFoo ObjectReturningProperty { get; }

        public int this[int i] => 1;

        public IFoo this[int i, int y] => null;

        internal virtual IFoo InternalObjectReturningMethod() => null;

        internal virtual IFoo InternalObjectReturningProperty => null;

        internal virtual IFoo this[int x, int y, int z] => null;
    }
}