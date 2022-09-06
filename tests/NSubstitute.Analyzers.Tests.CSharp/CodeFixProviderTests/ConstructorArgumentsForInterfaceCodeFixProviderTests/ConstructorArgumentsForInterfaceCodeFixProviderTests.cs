using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.ConstructorArgumentsForInterfaceCodeFixProviderTests;

public class ConstructorArgumentsForInterfaceCodeFixProviderTests : CSharpCodeFixVerifier, IConstructorArgumentsForInterfaceCodeFixVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SubstituteAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new ConstructorArgumentsForInterfaceCodeFixProvider();

    [Fact]
    public async Task RemovesInvocationArguments_WhenGenericFor_UsedWithParametersForInterface()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>(1, 2, 3);
            var otherSubstitute = NSubstitute.Substitute.For<IFoo>(new [] { 1, 2, 3 });
            var yetAnotherSubstitute = NSubstitute.Substitute.For<IFoo>(constructorArguments: new [] { 1, 2, 3 });
        }
    }
}";
        var newSource = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            var otherSubstitute = NSubstitute.Substitute.For<IFoo>();
            var yetAnotherSubstitute = NSubstitute.Substitute.For<IFoo>();
        }
    }
}";

        await VerifyFix(source, newSource);
    }

    [Fact]
    public async Task RemovesInvocationArguments_WhenNonGenericFor_UsedWithParametersForInterface()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For(new [] { typeof(IFoo) }, new object[] { 1 });
            var otherSubstitute = NSubstitute.Substitute.For(typesToProxy: new [] { typeof(IFoo) }, constructorArguments: new object[] { 1 });
            var yetAnotherSubstitute = NSubstitute.Substitute.For(constructorArguments: new object[] { 1 }, typesToProxy: new [] { typeof(IFoo) });
        }
    }
}";
        var newSource = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For(new [] { typeof(IFoo) }, null);
            var otherSubstitute = NSubstitute.Substitute.For(typesToProxy: new [] { typeof(IFoo) }, constructorArguments: null);
            var yetAnotherSubstitute = NSubstitute.Substitute.For(constructorArguments: null, typesToProxy: new [] { typeof(IFoo) });
        }
    }
}";
        await VerifyFix(source, newSource);
    }

    [Fact]
    public async Task RemovesInvocationArguments_WhenSubstituteFactoryCreate_UsedWithParametersForInterface()
    {
        var source = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(IFoo)}, new object[] { 1 });
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(IFoo)}, constructorArguments: new object[] { 1 });
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: new object[] { 1 }, typesToProxy: new[] {typeof(IFoo)});
        }
    }
}";
        var newSource = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(IFoo)}, null);
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(IFoo)}, constructorArguments: null);
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {typeof(IFoo)});
        }
    }
}";
        await VerifyFix(source, newSource);
    }
}