using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class CallInfoAnalyzer : AbstractCallInfoAnalyzer
{
    public CallInfoAnalyzer()
        : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance, CallInfoCallFinder.Instance, SubstitutionNodeFinder.Instance)
    {
    }

    protected override bool CanCast(Compilation compilation, ITypeSymbol sourceSymbol, ITypeSymbol destinationSymbol)
    {
        return compilation.ClassifyConversion(sourceSymbol, destinationSymbol).Exists;
    }

    protected override bool IsAssignableTo(Compilation compilation, ITypeSymbol fromSymbol, ITypeSymbol toSymbol)
    {
        var conversion = compilation.ClassifyConversion(fromSymbol, toSymbol);
        return conversion.Exists && conversion.IsExplicit == false && conversion.IsNumeric == false;
    }
}