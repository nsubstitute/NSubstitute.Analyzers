using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.Refactorings
{
    internal static class AddInternalsVisibleToAttributeRefactoring
    {
        public static Task<Document> RefactorAsync(Document document, CompilationUnitSyntax compilationUnitSyntax, CancellationToken cancellationToken = default)
        {
            var addAttributeLists = compilationUnitSyntax.AddAttributeLists(
                AttributeList(
                    AttributeTargetSpecifier(
                        Token(SyntaxKind.AssemblyKeyword)),
                    SingletonSeparatedList(
                        Attribute(
                            QualifiedName(
                                QualifiedName(
                                    QualifiedName(
                                        IdentifierName("System"),
                                        IdentifierName("Runtime")),
                                    IdentifierName("CompilerServices")),
                                IdentifierName("InternalsVisibleTo")),
                            AttributeArgumentList(
                                SingletonSeparatedList(
                                    AttributeArgument(
                                        LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal("DynamicProxyGenAssembly2")))))))));

            return document.ReplaceNodeAsync(compilationUnitSyntax, addAttributeLists, CancellationToken.None);
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