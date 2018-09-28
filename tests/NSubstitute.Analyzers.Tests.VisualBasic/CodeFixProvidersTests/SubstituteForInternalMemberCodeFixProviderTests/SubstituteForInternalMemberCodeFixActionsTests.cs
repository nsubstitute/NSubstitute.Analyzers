using System;
using System.Collections.Generic;
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

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.SubstituteForInternalMemberCodeFixProviderTests
{
    public class SubstituteForInternalMemberCodeFixActionsTests : VisualBasicCodeFixActionsVerifier, ISubstituteForInternalMemberCodeFixActionsVerifier
    {
        [Fact]
        public async Task CreatesCorrectCodeFixActions_WhenSourceForInternalType_IsAvailable()
        {
            var source = @"Imports NSubstitute.Core

Namespace MyNamespace
    Namespace MyInnerNamespace
        Friend Class Foo
        End Class

        Public Class FooTests
            Public Sub Test()
                Dim substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(Foo)}, Nothing)
            End Sub
        End Class
    End Namespace
End Namespace
";
            await VerifyCodeActions(source, "Append InternalsVisibleTo attribute");
        }

        [Fact]
        public async Task Does_Not_CreateCodeFixActions_WhenType_IsNotInternal()
        {
            var source = @"Imports NSubstitute.Core

Namespace MyNamespace
    Namespace MyInnerNamespace
        Public Class Foo
        End Class

        Public Class FooTests
            Public Sub Test()
                Dim substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(Foo)}, Nothing)
            End Sub
        End Class
    End Namespace
End Namespace
";
            await VerifyCodeActions(source);
        }

        [Fact]
        public async Task Does_Not_CreateCodeFixActions_WhenSourceForInternalType_IsNotAvailable()
        {
            var source = @"Imports NSubstitute.Core
Imports ExternalNamespace

Namespace MyNamespace
    Namespace MyInnerNamespace
        Public Class FooTests
            Public Sub Test()
                Dim substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(InternalFoo)}, Nothing)
            End Sub
        End Class
    End Namespace
End Namespace
";
            await VerifyCodeActions(source);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new SubstituteForInternalMemberCodeFixProvider();
        }

        protected override IEnumerable<MetadataReference> GetAdditionalMetadataReferences()
        {
            return new[] { GetInternalLibraryMetadataReference() };
        }

        private static PortableExecutableReference GetInternalLibraryMetadataReference()
        {
            var syntaxTree = VisualBasicSyntaxTree.ParseText($@"
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""{TestProjectName}"")>
Namespace ExternalNamespace
    Friend Class InternalFoo
    End Class
End Namespace
");

            var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
            var compilationOptions = new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation = VisualBasicCompilation.Create("Internal", new[] { syntaxTree }, references, compilationOptions);

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
}