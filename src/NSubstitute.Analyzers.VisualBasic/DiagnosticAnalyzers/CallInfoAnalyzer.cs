using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class CallInfoAnalyzer : AbstractCallInfoAnalyzer
{
    public CallInfoAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, CallInfoFinder.Instance, SubstitutionOperationFinder.Instance)
    {
    }

    protected override bool CanCast(Compilation compilation, ITypeSymbol sourceSymbol, ITypeSymbol destinationSymbol)
    {
        return compilation.ClassifyConversion(sourceSymbol, destinationSymbol).Exists;
    }

    protected override bool IsAssignableTo(Compilation compilation, ITypeSymbol fromSymbol, ITypeSymbol toSymbol)
    {
        var conversion = compilation.ClassifyConversion(fromSymbol, toSymbol);
        return conversion.Exists && conversion.IsNarrowing == false && conversion.IsNumeric == false;
    }
}