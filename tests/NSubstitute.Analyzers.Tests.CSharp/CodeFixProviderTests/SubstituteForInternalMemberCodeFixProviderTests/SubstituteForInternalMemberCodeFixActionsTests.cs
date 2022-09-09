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

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SubstituteForInternalMemberCodeFixProviderTests;

public class SubstituteForInternalMemberCodeFixActionsTests : CSharpCodeFixActionsVerifier, ISubstituteForInternalMemberCodeFixActionsVerifier
{
    private static readonly MetadataReference[] AdditionalMetadataReferences =
    {
        GetInternalLibraryMetadataReference()
    };

    public SubstituteForInternalMemberCodeFixActionsTests()
        : base(CSharpWorkspaceFactory.Default.WithAdditionalMetadataReferences(AdditionalMetadataReferences))
    {
    }

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SubstituteAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new SubstituteForInternalMemberCodeFixProvider();

    [Fact]
    public async Task CreatesCorrectCodeFixActions_WhenSourceForInternalType_IsAvailable()
    {
        var source = @"using NSubstitute.Core;
namespace MyNamespace
{
    namespace MyInnerNamespace
    {
        internal class Foo
        {
        }

        public class FooTests
        {
            public void Test()
            {
                var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
            }
        }
    }
}";
        await VerifyCodeActions(source, "Add InternalsVisibleTo attribute");
    }

    [Fact]
    public async Task Does_Not_CreateCodeFixActions_WhenType_IsNotInternal()
    {
        var source = @"using NSubstitute.Core;
namespace MyNamespace
{
    namespace MyInnerNamespace
    {
        public class Foo
        {
        }

        public class FooTests
        {
            public void Test()
            {
                var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
            }
        }
    }
}";
        await VerifyCodeActions(source);
    }

    [Fact]
    public async Task Does_Not_CreateCodeFixActions_WhenSourceForInternalType_IsNotAvailable()
    {
        var source = @"using NSubstitute.Core;
using ExternalNamespace;
namespace MyNamespace
{
    namespace MyInnerNamespace
    {
        public class FooTests
        {
            public void Test()
            {
                var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(InternalFoo)}, null);
            }
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
    internal class InternalFoo
    {{
    }}
}}");

        var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var compilation = CSharpCompilation.Create("Internal", new[] { syntaxTree }, references, compilationOptions);

        using var ms = new MemoryStream();
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