using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SubstituteForInternalMemberCodeFixProviderTests
{
    public class SubstituteForInternalMemberCodeFixProviderTests : CSharpCodeFixVerifier
    {
        [Fact]
        public async Task Foo()
        {
            var oldSource = @"using NSubstitute;
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo) }, null);
        }
    }
}";
            var newSource = @"using NSubstitute;
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo) }, null);
        }
    }
}";
            await VerifyFix(oldSource, newSource);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new SubstituteForInternalMemberCodeFixProvider();
        }
    }
}