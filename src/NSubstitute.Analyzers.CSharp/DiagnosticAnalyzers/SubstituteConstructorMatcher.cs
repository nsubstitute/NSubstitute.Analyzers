using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    public class SubstituteConstructorMatcher : AbstractSubstituteConstructorMatcher
    {
        protected override bool ClasifyConversion(Compilation compilation, ITypeSymbol source, ITypeSymbol destination)
        {
            var conversion = compilation.ClassifyConversion(source, destination);

            return conversion.Exists && conversion.IsImplicit;
        }
    }
}