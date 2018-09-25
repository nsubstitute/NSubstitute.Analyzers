using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    internal class SubstituteForInternalMemberCodeFixProvider : AbstractSubstituteForInternalMemberCodeFixProvider<InvocationExpressionSyntax, ExpressionSyntax, CompilationUnitSyntax>
    {
        protected override AbstractSubstituteProxyAnalysis<InvocationExpressionSyntax, ExpressionSyntax> GetSubstituteProxyAnalysis()
        {
            return new SubstituteProxyAnalysis();
        }

        protected override ICompilationUnitSyntax Annotate(CompilationUnitSyntax node)
        {
            var classDeclarationSyntax = node as ClassDeclarationSyntax;
            var compilationUnitSyntax = classDeclarationSyntax.Parent.Parent as CompilationUnitSyntax;

            return compilationUnitSyntax.WithAttributeLists(
                SingletonList(
                    AttributeList(
                            SingletonSeparatedList(
                                Attribute(
                                        IdentifierName("InternalsVisibleTo"))
                                    .WithArgumentList(
                                        AttributeArgumentList(
                                            SingletonSeparatedList(
                                                AttributeArgument(
                                                    LiteralExpression(
                                                        SyntaxKind.StringLiteralExpression,
                                                        Literal("DynamicProxyGenAssembly2"))))))))
                        .WithTrailingTrivia(CarriageReturn)
                        .WithTarget(
                            AttributeTargetSpecifier(
                                Token(SyntaxKind.AssemblyKeyword)))));
        }
    }
}