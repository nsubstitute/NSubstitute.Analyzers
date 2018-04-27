using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo<T>
    {
        int this[int x] { get; }
    }

    public class Foo<T> : IFoo<T>
    {
        public int Bar<TT>()
        {
            return 1;
        }

        public int this[int x] => throw new System.NotImplementedException();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            substitute[1].Returns<int>(1);
        }
    }
}