using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal class SubstituteForInternalMemberCodeFixProvider : AbstractSubstituteForInternalMemberCodeFixProvider<InvocationExpressionSyntax, ExpressionSyntax, CompilationUnitSyntax>
    {
        protected override AbstractSubstituteProxyAnalysis<InvocationExpressionSyntax, ExpressionSyntax>
            GetSubstituteProxyAnalysis()
        {
            return new SubstituteProxyAnalysis();
        }

        protected override CompilationUnitSyntax AppendInternalsVisibleToAttribute(CompilationUnitSyntax compilationUnitSyntax)
        {
            return compilationUnitSyntax.AddAttributes(
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
        }
    }
}