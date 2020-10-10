using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal interface ICallInfoFinder
    {
        CallInfoContext GetCallInfoContext(SemanticModel semanticModel, SyntaxNode syntaxNode);
    }
}