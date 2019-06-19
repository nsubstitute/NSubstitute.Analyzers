using NSubstitute.Core;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models
{
    public class Foo : IFoo
    {
        public void VoidReturningMethod()
        {
        }

        public void VoidReturningMethodWithArguments(int a, int b, decimal c)
        {
        }

        public IFoo ObjectReturningMethod() => null;

        public IFoo ObjectReturningMethodWithArguments(int a, int b, decimal c) => null;

        public IFoo ObjectReturningMethodWithRefArguments(ref int a, ref int b, ref decimal c) => null;

        internal virtual IFoo InternalObjectReturningMethod() => null;

        internal virtual IFoo InternalObjectReturningProperty => null;

        public ConfiguredCall CallInfoReturningMethod() => null;

        public ConfiguredCall ConfiguredCallReturningProperty { get; }

        public int IntReturningProperty { get; }

        public IFoo ObjectReturningProperty { get; }

        public int this[int a] => 1;

        public IFoo this[int a, int b] => null;

        public ConfiguredCall this[int a, int b, int c, int d] => null;

        internal virtual IFoo this[int a, int b, int c] => null;
    }
}