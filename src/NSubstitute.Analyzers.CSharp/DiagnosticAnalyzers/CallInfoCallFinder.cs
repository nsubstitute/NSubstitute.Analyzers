using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class CallInfoCallFinder : AbstractCallInfoFinder<InvocationExpressionSyntax, ElementAccessExpressionSyntax>
    {
        public override CallInfoContext<InvocationExpressionSyntax, ElementAccessExpressionSyntax> GetCallInfoContext(SemanticModel semanticModel, SyntaxNode syntaxNode)
        {
            var visitor = new CallInfoVisitor(semanticModel);
            visitor.Visit(syntaxNode);

            return new CallInfoContext<InvocationExpressionSyntax, ElementAccessExpressionSyntax>(visitor.ArgAtInvocations, visitor.ArgInvocations, visitor.DirectIndexerAccesses);
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

                if (symbolInfo.Symbol != null && symbolInfo.Symbol.ContainingType.ToString().Equals(MetadataNames.NSubstituteCallInfoFullTypeName))
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
                if (symbolInfo != null && symbolInfo.ContainingType.ToString().Equals(MetadataNames.NSubstituteCallInfoFullTypeName))
                {
                    DirectIndexerAccesses.Add(node);
                }

                base.VisitElementAccessExpression(node);
            }
        }
    }
}