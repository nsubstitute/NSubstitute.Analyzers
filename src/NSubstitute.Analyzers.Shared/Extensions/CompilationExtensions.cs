#nullable enable
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class CompilationExtensions
{
    public static SemanticModel? TryGetSemanticModel(this Compilation compilation, SyntaxTree syntaxTree) =>
        compilation.ContainsSyntaxTree(syntaxTree) ? compilation.GetSemanticModel(syntaxTree) : null;
}