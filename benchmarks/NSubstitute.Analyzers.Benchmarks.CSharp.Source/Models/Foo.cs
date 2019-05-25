namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.Models
{
    public class Foo : IFoo
    {
        public void VoidReturningMethod()
        {
        }

        public object ObjectReturningMethod() => null;

        public virtual void VoidReturningVirtualMethod()
        {
        }

        public void VoidReturningMethodWithArguments(int x, int y, decimal z)
        {
        }

        public object ObjectReturningMethodWithArguments(int x, int y, decimal z) => null;

        public object ObjectReturningMethodWithRefArguments(ref int x, ref int y, ref decimal z) => null;

        public int Property { get; }

        public object ObjectReturningProperty { get; }

        public int this[int i] => 1;

        public object this[int i, int y] => null;

        internal virtual object InternalObjectReturningMethod() => null;

        internal virtual object InternalObjectReturningProperty => null;

        internal virtual object this[int x, int y, int z] => null;
    }
}