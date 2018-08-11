using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    }
}