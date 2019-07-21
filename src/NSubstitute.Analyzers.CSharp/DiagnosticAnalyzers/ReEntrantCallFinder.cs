using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class ReEntrantCallFinder : AbstractReEntrantCallFinder
    {
        public static ReEntrantCallFinder Instance { get; } = new ReEntrantCallFinder();

        private ReEntrantCallFinder()
        {
        }

        protected override ImmutableList<ISymbol> GetReEntrantSymbols(Compilation compilation, SemanticModel semanticModel, SyntaxNode originatingExpression, SyntaxNode rootNode)
        {
            var visitor = new ReEntrantCallVisitor(this, compilation, semanticModel, originatingExpression);
            visitor.Visit(rootNode);
            return visitor.InvocationSymbols;
        }

        private class ReEntrantCallVisitor : CSharpSyntaxWalker
        {
            private readonly ReEntrantCallFinder _reEntrantCallFinder;
            private readonly Compilation _compilation;
            private readonly SemanticModel _semanticModel;
            private readonly HashSet<SyntaxNode> _visitedNodes = new HashSet<SyntaxNode>();
            private readonly List<ISymbol> _invocationSymbols = new List<ISymbol>();

            public ImmutableList<ISymbol> InvocationSymbols => _invocationSymbols.ToImmutableList();

            public ReEntrantCallVisitor(ReEntrantCallFinder reEntrantCallFinder, Compilation compilation, SemanticModel semanticModel, SyntaxNode originatingExpression)
            {
                _reEntrantCallFinder = reEntrantCallFinder;
                _compilation = compilation;
                _semanticModel = semanticModel;
                _visitedNodes.Add(originatingExpression);
            }

            public override void VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                if (_visitedNodes.Contains(node) == false && _compilation.ContainsSyntaxTree(node.SyntaxTree))
                {
                    var semanticModel = _reEntrantCallFinder.GetSemanticModel(_compilation, _semanticModel, node);
                    var symbolInfo = semanticModel.GetSymbolInfo(node);
                    if (_reEntrantCallFinder.IsInnerReEntryLikeMethod(semanticModel, symbolInfo.Symbol))
                    {
                        _invocationSymbols.Add(symbolInfo.Symbol);
                    }
                }

                base.VisitInvocationExpression(node);
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

            public override void DefaultVisit(SyntaxNode node)
            {
                VisitRelatedSymbols(node);
                base.DefaultVisit(node);
            }

            private void VisitRelatedSymbols(SyntaxNode syntaxNode)
            {
                if ((syntaxNode.IsKind(SyntaxKind.IdentifierName) ||
                     syntaxNode.IsKind(SyntaxKind.ElementAccessExpression) ||
                     syntaxNode.IsKind(SyntaxKind.SimpleMemberAccessExpression)) && _visitedNodes.Contains(syntaxNode) == false)
                {
                    _visitedNodes.Add(syntaxNode);
                    foreach (var relatedNode in _reEntrantCallFinder.GetRelatedNodes(_compilation, _semanticModel, syntaxNode).Where(node => _visitedNodes.Contains(node) == false))
                    {
                        Visit(relatedNode);
                    }
                }
            }
        }
    }
}