using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class CallInfoContext
    {
        public List<ElementAccessExpressionSyntax> IndexerAccesses { get; }

        public List<InvocationExpressionSyntax> ArgAtInvocations { get; }

        public List<InvocationExpressionSyntax> ArgInvocations { get; }

        public CallInfoContext(List<InvocationExpressionSyntax> argAtInvocations, List<InvocationExpressionSyntax> argInvocations, List<ElementAccessExpressionSyntax> indexerAccesses)
        {
            IndexerAccesses = indexerAccesses;
            ArgAtInvocations = argAtInvocations;
            ArgInvocations = argInvocations;
        }
    }
}