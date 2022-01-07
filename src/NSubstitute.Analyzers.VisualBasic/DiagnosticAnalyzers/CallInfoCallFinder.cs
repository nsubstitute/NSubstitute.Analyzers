using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

internal class CallInfoCallFinder : AbstractCallInfoFinder<InvocationExpressionSyntax, InvocationExpressionSyntax>
{
    public static CallInfoCallFinder Instance { get; } = new CallInfoCallFinder();

    private CallInfoCallFinder()
    {
    }

    protected override CallInfoContext<InvocationExpressionSyntax, InvocationExpressionSyntax> GetCallInfoContextInternal(SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        var visitor = new CallInfoVisitor(semanticModel);
        visitor.Visit(syntaxNode);

        return new CallInfoContext<InvocationExpressionSyntax, InvocationExpressionSyntax>(
            argAtInvocations: visitor.ArgAtInvocations,
            argInvocations: visitor.ArgInvocations,
            indexerAccesses: visitor.DirectIndexerAccesses);
    }

    private class CallInfoVisitor : VisualBasicSyntaxWalker
    {
        private readonly SemanticModel _semanticModel;

        public List<InvocationExpressionSyntax> ArgAtInvocations { get; }

        public List<InvocationExpressionSyntax> ArgInvocations { get; }

        public List<InvocationExpressionSyntax> DirectIndexerAccesses { get; }

        public CallInfoVisitor(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            DirectIndexerAccesses = new List<InvocationExpressionSyntax>();
            ArgAtInvocations = new List<InvocationExpressionSyntax>();
            ArgInvocations = new List<InvocationExpressionSyntax>();
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var symbol = _semanticModel.GetSymbolInfo(node).Symbol;

            if (symbol != null && symbol.ContainingType.IsCallInfoSymbol())
            {
                switch (symbol.Name)
                {
                    case MetadataNames.CallInfoArgAtMethod:
                        ArgAtInvocations.Add(node);
                        break;
                    case MetadataNames.CallInfoArgMethod:
                        ArgInvocations.Add(node);
                        break;
                    case "Item":
                        DirectIndexerAccesses.Add(node);
                        break;
                }
            }

            if (symbol == null)
            {
                var expressionSymbol = _semanticModel.GetSymbolInfo(node.Expression).Symbol;

                if (expressionSymbol != null && expressionSymbol.ContainingType.IsCallInfoSymbol())
                {
                    DirectIndexerAccesses.Add(node);
                }
            }

            base.VisitInvocationExpression(node);
        }
    }
}