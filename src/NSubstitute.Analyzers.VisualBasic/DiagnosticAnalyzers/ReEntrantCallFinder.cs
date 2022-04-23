using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

internal class ReEntrantCallFinder : AbstractReEntrantCallFinder<InvocationExpressionSyntax, IdentifierNameSyntax>
{
    public static ReEntrantCallFinder Instance { get; } = new ReEntrantCallFinder(SubstitutionNodeFinder.Instance);

    private ReEntrantCallFinder(ISubstitutionNodeFinder substitutionNodeFinder)
        : base(substitutionNodeFinder)
    {
    }

    protected override ImmutableList<ISymbol> GetReEntrantSymbols(Compilation compilation, SemanticModel semanticModel, SyntaxNode originatingExpression, SyntaxNode rootNode)
    {
        var visitor = new ReEntrantCallVisitor(this, compilation, semanticModel);
        visitor.Visit(rootNode);
        return visitor.InvocationSymbols;
    }

    protected override IEnumerable<InvocationExpressionSyntax> GetPotentialOtherSubstituteInvocations(IEnumerable<SyntaxNode> nodes)
    {
        foreach (var node in nodes)
        {
            switch (node)
            {
                case InvocationExpressionSyntax invocationExpressionSyntax:
                    yield return invocationExpressionSyntax;
                    break;
                case ExpressionStatementSyntax expressionStatementSyntax when expressionStatementSyntax.Expression is InvocationExpressionSyntax invocationExpressionSyntax:
                    yield return invocationExpressionSyntax;
                    break;
                case ConstructorBlockSyntax constructorDeclarationSyntax:
                    foreach (var potentialPreviousReturnsLikeInvocation in GetPotentialOtherSubstituteInvocations(
                                 constructorDeclarationSyntax.ChildNodes()))
                    {
                        yield return potentialPreviousReturnsLikeInvocation;
                    }

                    break;
            }
        }
    }

    private class ReEntrantCallVisitor : VisualBasicSyntaxWalker
    {
        private readonly ReEntrantCallFinder _reEntrantCallFinder;
        private readonly Compilation _compilation;
        private readonly SemanticModel _semanticModel;
        private readonly HashSet<SyntaxNode> _visitedNodes = new HashSet<SyntaxNode>();
        private readonly List<ISymbol> _invocationSymbols = new List<ISymbol>();

        public ImmutableList<ISymbol> InvocationSymbols => _invocationSymbols.ToImmutableList();

        public ReEntrantCallVisitor(ReEntrantCallFinder reEntrantCallFinder, Compilation compilation, SemanticModel semanticModel)
        {
            _reEntrantCallFinder = reEntrantCallFinder;
            _compilation = compilation;
            _semanticModel = semanticModel;
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

        public override void VisitClassBlock(ClassBlockSyntax node)
        {
        }

        public override void VisitStructureBlock(StructureBlockSyntax node)
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

        public override void VisitAttributesStatement(AttributesStatementSyntax node)
        {
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
        }

        public override void VisitAttributeTarget(AttributeTargetSyntax node)
        {
        }

        public override void VisitImportsStatement(ImportsStatementSyntax node)
        {
        }

        public override void VisitEnumBlock(EnumBlockSyntax node)
        {
        }

        public override void VisitEnumStatement(EnumStatementSyntax node)
        {
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
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

        public override void VisitTypeParameterMultipleConstraintClause(TypeParameterMultipleConstraintClauseSyntax node)
        {
        }

        public override void VisitTypeParameterSingleConstraintClause(TypeParameterSingleConstraintClauseSyntax node)
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
                 syntaxNode.IsKind(SyntaxKind.SimpleMemberAccessExpression)) && _visitedNodes.Contains(syntaxNode) == false)
            {
                _visitedNodes.Add(syntaxNode);
                foreach (var relatedNode in _reEntrantCallFinder.GetRelatedNodes(_compilation, _semanticModel, syntaxNode).Where(node => _visitedNodes.Contains(node) == false))
                {
                    var currentNode = relatedNode;

                    // hack getting related symbols for method identifier doesnt return method block but method statement syntax
                    if (syntaxNode.IsKind(SyntaxKind.IdentifierName) && currentNode is MethodStatementSyntax && _visitedNodes.Contains(relatedNode.Parent) == false)
                    {
                        currentNode = relatedNode.Parent;
                    }

                    Visit(currentNode);
                }
            }
        }
    }
}