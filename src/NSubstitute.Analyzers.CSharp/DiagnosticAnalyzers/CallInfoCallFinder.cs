using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

internal class CallInfoCallFinder : AbstractCallInfoFinder
{
    public static CallInfoCallFinder Instance { get; } = new CallInfoCallFinder();

    private CallInfoCallFinder()
    {
    }

    protected override CallInfoContext GetCallInfoContextInternal(SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        var visitor = new CallInfoVisitor(semanticModel);
        visitor.Visit(syntaxNode);

        return new CallInfoContext(
            argAtInvocations: visitor.ArgAtInvocations,
            argInvocations: visitor.ArgInvocations,
            indexerAccesses: visitor.DirectIndexerAccesses);
    }

    private class CallInfoVisitor : CSharpSyntaxWalker
    {
        private readonly SemanticModel _semanticModel;

        public List<InvocationExpressionSyntax> ArgAtInvocations { get; }

        public List<InvocationExpressionSyntax> ArgInvocations { get; }

        public List<ElementAccessExpressionSyntax> DirectIndexerAccesses { get; }

        public CallInfoVisitor(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            DirectIndexerAccesses = new List<ElementAccessExpressionSyntax>();
            ArgAtInvocations = new List<InvocationExpressionSyntax>();
            ArgInvocations = new List<InvocationExpressionSyntax>();
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(node);

            if (symbolInfo.Symbol != null && symbolInfo.Symbol.ContainingType.IsCallInfoSymbol())
            {
                switch (symbolInfo.Symbol.Name)
                {
                    case MetadataNames.CallInfoArgAtMethod:
                        ArgAtInvocations.Add(node);
                        break;
                    case MetadataNames.CallInfoArgMethod:
                        ArgInvocations.Add(node);
                        break;
                }
            }

            base.VisitInvocationExpression(node);
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            var symbolInfo = ModelExtensions.GetSymbolInfo(_semanticModel, node).Symbol ?? ModelExtensions.GetSymbolInfo(_semanticModel, node.Expression).Symbol;
            if (symbolInfo != null && symbolInfo.ContainingType.IsCallInfoSymbol())
            {
                DirectIndexerAccesses.Add(node);
            }

            base.VisitElementAccessExpression(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
        }

        public override void VisitTrivia(SyntaxTrivia trivia)
        {
        }

        public override void VisitLeadingTrivia(SyntaxToken token)
        {
        }

        public override void VisitTrailingTrivia(SyntaxToken token)
        {
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
        }

        public override void VisitAttributeArgument(AttributeArgumentSyntax node)
        {
        }

        public override void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
        }

        public override void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
        }

        public override void VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
        {
        }

        public override void VisitTypeArgumentList(TypeArgumentListSyntax node)
        {
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
        }
    }
}