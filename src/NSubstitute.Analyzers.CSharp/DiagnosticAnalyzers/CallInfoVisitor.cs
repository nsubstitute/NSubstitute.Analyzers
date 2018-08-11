using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class CallInfoVisitor : CSharpSyntaxWalker
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

            if (symbolInfo.Symbol != null &&
                symbolInfo.Symbol.ContainingType.ToString().Equals(MetadataNames.NSubstituteCoreFullTypeName))
            {
                if (symbolInfo.Symbol.Name == MetadataNames.CallInfoArgAtMethod)
                {
                    ArgAtInvocations.Add(node);
                }

                if (symbolInfo.Symbol.Name == MetadataNames.CallInfoArgMethod)
                {
                    ArgInvocations.Add(node);
                }
            }

            base.VisitInvocationExpression(node);
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(node).Symbol ?? _semanticModel.GetSymbolInfo(node.Expression).Symbol;
            if (symbolInfo != null && symbolInfo.ContainingType.ToString().Equals(MetadataNames.NSubstituteCoreFullTypeName))
            {
                DirectIndexerAccesses.Add(node);
            }

            base.VisitElementAccessExpression(node);
        }
    }
}