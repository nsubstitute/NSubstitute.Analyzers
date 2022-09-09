using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.InternalSetupSpecificationCodeFixProviderTests;

public class InternalSetupSpecificationCodeFixActionsTests : VisualBasicCodeFixActionsVerifier, IInternalSetupSpecificationCodeFixActionsVerifier
{
    private static readonly MetadataReference[] AdditionalMetadataReferences =
    {
        GetInternalLibraryMetadataReference()
    };

    public InternalSetupSpecificationCodeFixActionsTests()
        : base(VisualBasicWorkspaceFactory.Default.WithAdditionalMetadataReferences(AdditionalMetadataReferences))
    {
    }

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new InternalSetupSpecificationCodeFixProvider();

    [Fact]
    public async Task CreateCodeActions_InProperOrder()
    {
        var source = @"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Friend Overridable Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = substitute.Bar().Returns(1)
        End Sub
    End Class
End Namespace
";
        await VerifyCodeActions(source, "Add protected modifier", "Replace friend with public modifier", "Add InternalsVisibleTo attribute");
    }

    [Fact]
    public async Task DoesNotCreateCodeActions_WhenSymbol_DoesNotBelongToCompilation()
    {
        var source = @"Imports NSubstitute
Imports ExternalNamespace

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of InternalFoo)()
            Dim x = substitute.Bar().Returns(1)
        End Sub
    End Class
End Namespace";

        await VerifyCodeActions(source);
    }

    private static MetadataReference GetInternalLibraryMetadataReference()
    {
        var syntaxTree = VisualBasicSyntaxTree.ParseText($@"Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""{Shared.WorkspaceFactory.DefaultProjectName}"")>
Namespace ExternalNamespace
    Public Class InternalFoo
        Friend Overridable Function Bar() As Integer
            Return 1
        End Function
    End Class
End Namespace
");

        var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
        var compilationOptions = new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var compilation = VisualBasicCompilation.Create("Internal", new[] { syntaxTree }, references, compilationOptions);

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