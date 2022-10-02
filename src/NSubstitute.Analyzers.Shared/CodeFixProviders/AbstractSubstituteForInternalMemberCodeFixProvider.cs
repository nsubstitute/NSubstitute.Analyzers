using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractSubstituteForInternalMemberCodeFixProvider<TCompilationUnitSyntax> : AbstractSuppressDiagnosticsCodeFixProvider where TCompilationUnitSyntax : SyntaxNode
{
    private readonly ISubstituteProxyAnalysis _substituteProxyAnalysis;

    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.SubstituteForInternalMember);

    protected AbstractSubstituteForInternalMemberCodeFixProvider(ISubstituteProxyAnalysis substituteProxyAnalysis)
    {
        _substituteProxyAnalysis = substituteProxyAnalysis;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var invocationExpression = root.FindNode(context.Span, getInnermostNodeForTie: true);
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

        if (semanticModel.GetOperation(invocationExpression) is not IInvocationOperation invocationOperation)
        {
            return;
        }

        var syntaxReference = GetDeclaringSyntaxReference(invocationOperation);

        if (syntaxReference == null)
        {
            return;
        }

        var syntaxNode = await syntaxReference.GetSyntaxAsync();
        var compilationUnitSyntax = FindCompilationUnitSyntax(syntaxNode);

        if (compilationUnitSyntax == null)
        {
            return;
        }

        RegisterCodeFix(context, compilationUnitSyntax);
    }

    protected abstract void RegisterCodeFix(CodeFixContext context, TCompilationUnitSyntax compilationUnitSyntax);

    private SyntaxReference GetDeclaringSyntaxReference(IInvocationOperation invocationOperation)
    {
        var actualProxyTypeSymbol = _substituteProxyAnalysis.GetActualProxyTypeSymbol(invocationOperation);
        return actualProxyTypeSymbol.DeclaringSyntaxReferences.FirstOrDefault();
    }

    private TCompilationUnitSyntax FindCompilationUnitSyntax(SyntaxNode syntaxNode)
    {
        return syntaxNode.Ancestors().OfType<TCompilationUnitSyntax>().LastOrDefault();
    }
}