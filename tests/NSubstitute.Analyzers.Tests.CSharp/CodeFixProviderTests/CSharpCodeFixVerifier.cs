using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests
{
    public abstract class CSharpCodeFixVerifier : CodeFixVerifier
    {
        protected override string Language { get; } = LanguageNames.CSharp;

        protected override CompilationOptions GetCompilationOptions()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        }

        protected override IEnumerable<MetadataReference> GetAdditionalMetadataReferences()
        {
            return new MetadataReference[]
            {
                Foo()
            };
        }

        private static PortableExecutableReference Foo()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace ExternalNamespace
{
    internal class InternalFoo
    {
    }
}");

            var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation = CSharpCompilation.Create("Internal", new[] { syntaxTree }, references: references, options: compilationOptions);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                ms.Seek(0, SeekOrigin.Begin);
                return MetadataReference.CreateFromStream(ms);
            }
        }
    }
}