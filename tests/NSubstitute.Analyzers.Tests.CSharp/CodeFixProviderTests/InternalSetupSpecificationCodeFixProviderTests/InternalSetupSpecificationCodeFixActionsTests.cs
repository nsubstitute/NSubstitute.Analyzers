using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.InternalSetupSpecificationCodeFixProviderTests
{
    public class InternalSetupSpecificationCodeFixActionsTests : CSharpCodeFixActionsVerifier, IInternalSetupSpecificationCodeFixActionsVerifier
    {
        [Fact]
        public async Task CreateCodeActions_InProperOrder()
        {
            var source = @"using NSubstitute;
using System.Runtime.CompilerServices;

namespace MyNamespace
{
    public class Foo
    {
        internal virtual int Bar()
        {
            return 1;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.Bar().Returns(1);
        }
    }
}";
            await VerifyCodeActions(source, "Add protected modifier", "Replace internal with public modifier", "Add InternalsVisibleTo attribute");
        }

        [Fact]
        public async Task DoesNotCreateCodeActions_WhenSymbol_DoesNotBelongToCompilation()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<object>();
            var x = substitute.ToString().Returns(string.Empty);
        }
    }
}";
            await VerifyCodeActions(source);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonSubstitutableMemberAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new InternalSetupSpecificationCodeFixProvider();
        }
    }
}