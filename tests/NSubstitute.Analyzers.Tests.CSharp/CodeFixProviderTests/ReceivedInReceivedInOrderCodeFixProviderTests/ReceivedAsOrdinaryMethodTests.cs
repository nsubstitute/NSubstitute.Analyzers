using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.ReceivedInReceivedInOrderCodeFixProviderTests;

public class ReceivedAsOrdinaryMethodTests : ReceivedInReceivedInOrderCodeFixVerifier
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
                SubstituteExtensions.Received(substitute).Bar();
                SubstituteExtensions.Received(substitute).Bar(Arg.Any<int>());
                SubstituteExtensions.ReceivedWithAnyArgs(substitute).Bar(Arg.Any<int>());
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
                SubstituteExtensions.Received(substitute, 1).Bar();
                SubstituteExtensions.Received(substitute: substitute, requiredNumberOfCalls: 1).Bar();
                SubstituteExtensions.Received(requiredNumberOfCalls: 1, substitute: substitute).Bar();
                SubstituteExtensions.Received(substitute, 1).Bar(Arg.Any<int>());
                SubstituteExtensions.Received(substitute: substitute, requiredNumberOfCalls: 1).Bar(Arg.Any<int>());
                SubstituteExtensions.Received(requiredNumberOfCalls: 1, substitute: substitute).Bar(Arg.Any<int>());
                SubstituteExtensions.ReceivedWithAnyArgs(substitute, 1).Bar(Arg.Any<int>());
                SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredNumberOfCalls: 1).Bar(Arg.Any<int>());
                SubstituteExtensions.ReceivedWithAnyArgs(requiredNumberOfCalls: 1, substitute: substitute).Bar(Arg.Any<int>());
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
                substitute.Bar();
                substitute.Bar();
                substitute.Bar(Arg.Any<int>());
                substitute.Bar(Arg.Any<int>());
                substitute.Bar(Arg.Any<int>());
                substitute.Bar(Arg.Any<int>());
                substitute.Bar(Arg.Any<int>());
                substitute.Bar(Arg.Any<int>());
            });
        }
    }
}";
        await VerifyFix(oldSource, newSource);
    }
}