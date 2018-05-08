using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class Foo
    {
        public int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Bar(), 1);
        }
    }
}