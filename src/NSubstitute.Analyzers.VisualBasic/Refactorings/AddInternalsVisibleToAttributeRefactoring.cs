using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;
using static Microsoft.CodeAnalysis.Editing.SyntaxGenerator;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.Refactorings
{
    internal static class AddInternalsVisibleToAttributeRefactoring
    {
        public static Task<Document> RefactorAsync(Document document, CompilationUnitSyntax compilationUnitSyntax, CancellationToken cancellationToken = default)
        {
            var attributeList = GetGenerator(document)
                .InternalVisibleToDynamicProxyAttributeList()
                .Cast<AttributeListSyntax>();

            var assemblyAttributes = attributeList.Attributes.Select(attr =>
                attr.WithTarget(AttributeTarget(Token(SyntaxKind.AssemblyKeyword))));

            attributeList = attributeList.WithAttributes(SeparatedList(assemblyAttributes));

            var updatedCompilationUnitSyntax =
                compilationUnitSyntax.AddAttributes(AttributesStatement(SingletonList(attributeList)));

            return document.ReplaceNodeAsync(compilationUnitSyntax, updatedCompilationUnitSyntax, cancellationToken);
        }

        public static void RegisterCodeFix(CodeFixContext context, Diagnostic diagnostic, CompilationUnitSyntax compilationUnitSyntax)
        {
            var codeAction = CodeAction.Create(
                "Add InternalsVisibleTo attribute",
                cancellationToken => RefactorAsync(context.Document, compilationUnitSyntax, cancellationToken),
                diagnostic.Id);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}