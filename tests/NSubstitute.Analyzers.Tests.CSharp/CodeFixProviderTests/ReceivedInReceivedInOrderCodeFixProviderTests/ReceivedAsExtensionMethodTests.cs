using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.ReceivedInReceivedInOrderCodeFixProviderTests;

public class ReceivedAsExtensionMethodTests : ReceivedInReceivedInOrderCodeFixVerifier
{
    [Fact]
    public override async Task RemovesReceivedChecks_WhenReceivedChecksHasNoArguments()
    {
        var oldSource = @"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar();
        int Bar(int x);
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder(() =>
            { 
                substitute.Received().Bar();
                substitute.Received().Bar(Arg.Any<int>());
                substitute.ReceivedWithAnyArgs().Bar(Arg.Any<int>());
            });
        }
    }
}";

        var newSource = @"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar();
        int Bar(int x);
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder(() =>
            { 
                substitute.Bar();
                substitute.Bar(Arg.Any<int>());
                substitute.Bar(Arg.Any<int>());
            });
        }
    }
}";
        await VerifyFix(oldSource, newSource);
    }

    [Fact]
    public override async Task RemovesReceivedChecks_WhenReceivedChecksHasArguments()
    {
        var oldSource = @"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar();
        int Bar(int x);
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder(() =>
            { 
                substitute.Received(1).Bar();
                substitute.Received(1).Bar(Arg.Any<int>());
            });
        }
    }
}";

        var newSource = @"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar();
        int Bar(int x);
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder(() =>
            { 
                substitute.Bar();
                substitute.Bar(Arg.Any<int>());
            });
        }
    }
}";
        await VerifyFix(oldSource, newSource);
    }
}