using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    public class SubstituteConstructorMatcher : AbstractSubstituteConstructorMatcher
    {
        protected override bool ClasifyConversion(Compilation compilation, ITypeSymbol source, ITypeSymbol destination)
        {
            var conversion = compilation.ClassifyConversion(source, destination);

            return conversion.Exists && conversion.IsNarrowing;
        }
    }
}