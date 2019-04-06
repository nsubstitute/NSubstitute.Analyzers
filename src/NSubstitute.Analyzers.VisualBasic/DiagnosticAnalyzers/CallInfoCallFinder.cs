using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    internal class CallInfoCallFinder : ICallInfoFinder<InvocationExpressionSyntax, InvocationExpressionSyntax>
    {
        public CallInfoContext<InvocationExpressionSyntax, InvocationExpressionSyntax> GetCallInfoContext(SemanticModel semanticModel, SyntaxNode syntaxNode)
        {
            var visitor = new CallInfoVisitor(semanticModel);
            visitor.Visit(syntaxNode);

            return new CallInfoContext<InvocationExpressionSyntax, InvocationExpressionSyntax>(visitor.ArgAtInvocations, visitor.ArgInvocations, visitor.DirectIndexerAccesses);
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

                if (symbol != null && symbol.ContainingType.ToString().Equals(MetadataNames.NSubstituteCallInfoFullTypeName))
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

                    if (expressionSymbol != null && expressionSymbol.ContainingType.ToString().Equals(MetadataNames.NSubstituteCallInfoFullTypeName))
                    {
                        DirectIndexerAccesses.Add(node);
                    }
                }

                base.VisitInvocationExpression(node);
            }
        }
    }
}