using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

internal sealed class SubstituteConstructorMatcher : AbstractSubstituteConstructorMatcher
{
    public static SubstituteConstructorMatcher Instance { get; } = new();

    private SubstituteConstructorMatcher()
    {
    }

    protected override bool IsConvertible(Compilation compilation, ITypeSymbol source, ITypeSymbol destination)
    {
        var conversion = compilation.ClassifyConversion(source, destination);

        return conversion.Exists && conversion.IsImplicit;
    }
}