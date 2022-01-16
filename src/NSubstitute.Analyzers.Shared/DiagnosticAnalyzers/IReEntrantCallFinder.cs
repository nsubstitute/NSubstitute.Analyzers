using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface IReEntrantCallFinder
{
    ImmutableList<ISymbol> GetReEntrantCalls(Compilation compilation, SemanticModel semanticModel, SyntaxNode originatingExpression, SyntaxNode rootNode);
}