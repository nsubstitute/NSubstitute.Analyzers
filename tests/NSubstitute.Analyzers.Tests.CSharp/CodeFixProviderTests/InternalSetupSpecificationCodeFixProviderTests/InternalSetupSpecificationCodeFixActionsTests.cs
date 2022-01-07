using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.InternalSetupSpecificationCodeFixProviderTests;

public class InternalSetupSpecificationCodeFixActionsTests : CSharpCodeFixActionsVerifier, IInternalSetupSpecificationCodeFixActionsVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new InternalSetupSpecificationCodeFixProvider();

    private static readonly MetadataReference[] AdditionalMetadataReferences =
    {
        GetInternalLibraryMetadataReference()
    };

    public InternalSetupSpecificationCodeFixActionsTests()
        : base(CSharpWorkspaceFactory.Default.WithAdditionalMetadataReferences(AdditionalMetadataReferences))
    {
    }

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
using ExternalNamespace;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<InternalFoo>();
            var x = substitute.Bar().Returns(1);
        }
    }
}";
        await VerifyCodeActions(source);
    }

    private static MetadataReference GetInternalLibraryMetadataReference()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText($@"
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""{Shared.WorkspaceFactory.DefaultProjectName}"")]
namespace ExternalNamespace
{{
    public class InternalFoo
    {{
        internal virtual int Bar()
        {{
            return 1;
        }}
    }}
}}");

        var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var compilation = CSharpCompilation.Create("Internal", new[] { syntaxTree }, references, compilationOptions);

        using (var ms = new MemoryStream())
        {
            var result = compilation.Emit(ms);

            if (result.Success == false)
            {
                var errors = result.Diagnostics.Where(diag => diag.IsWarningAsError || diag.Severity == DiagnosticSeverity.Error);
                throw new InvalidOperationException($"Internal library compilation failed: {string.Join(",", errors)}");
            }

            ms.Seek(0, SeekOrigin.Begin);
            return MetadataReference.CreateFromStream(ms);
        }
    }
}