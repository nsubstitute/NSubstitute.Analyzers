using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.Refactorings
{
    internal static class AddInternalsVisibleToAttributeRefactoring
    {
        public static Task<Document> RefactorAsync(Document document, CompilationUnitSyntax compilationUnitSyntax, CancellationToken cancellationToken = default)
        {
            var addAttributeLists = compilationUnitSyntax.AddAttributes(
                AttributesStatement(SingletonList(
                    AttributeList(SingletonSeparatedList(Attribute(
                        AttributeTarget(Token(SyntaxKind.AssemblyKeyword)),
                        QualifiedName(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("Runtime")),
                                IdentifierName("CompilerServices")),
                            IdentifierName("InternalsVisibleTo")),
                        ArgumentList(SingletonSeparatedList<ArgumentSyntax>(SimpleArgument(
                            LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal("DynamicProxyGenAssembly2")))))))))));

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