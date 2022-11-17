using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.Editing.SyntaxGenerator;

namespace NSubstitute.Analyzers.CSharp.Refactorings;

internal static class AddInternalsVisibleToAttributeRefactoring
{
    public static Task<Document> RefactorAsync(Document document, CompilationUnitSyntax compilationUnitSyntax, CancellationToken cancellationToken)
    {
        var attributeList = GetGenerator(document)
            .InternalVisibleToDynamicProxyAttributeList()
            .Cast<AttributeListSyntax>()
            .WithTarget(AttributeTargetSpecifier(
                Token(SyntaxKind.AssemblyKeyword)));

        var updatedCompilationUnitSyntax = compilationUnitSyntax.AddAttributeLists(attributeList);
        return document.ReplaceNodeAsync(compilationUnitSyntax, updatedCompilationUnitSyntax, cancellationToken);
    }

    public static void RegisterCodeFix(CodeFixContext context, CompilationUnitSyntax compilationUnitSyntax)
    {
        var codeAction = CodeAction.Create(
            "Add InternalsVisibleTo attribute",
            cancellationToken => RefactorAsync(context.Document, compilationUnitSyntax, cancellationToken));

        context.RegisterCodeFix(codeAction, context.Diagnostics);
    }
}