using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractSubstituteForInternalMemberCodeFixProvider<TInvocationExpressionSyntax, TCompilationUnitSyntax> : AbstractSuppressDiagnosticsCodeFixProvider
    where TInvocationExpressionSyntax : SyntaxNode
    where TCompilationUnitSyntax : SyntaxNode
{
    private readonly ISubstituteProxyAnalysis _substituteProxyAnalysis;

    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.SubstituteForInternalMember);

    protected AbstractSubstituteForInternalMemberCodeFixProvider(ISubstituteProxyAnalysis substituteProxyAnalysis)
    {
        _substituteProxyAnalysis = substituteProxyAnalysis;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.FirstOrDefault(diag => diag.Descriptor.Id == DiagnosticIdentifiers.SubstituteForInternalMember);
        if (diagnostic == null)
        {
            return;
        }

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var findNode = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
        if (!(findNode is TInvocationExpressionSyntax invocationExpression))
        {
            return;
        }

        var syntaxReference = await GetDeclaringSyntaxReference(context, invocationExpression);

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

        RegisterCodeFix(context, diagnostic, compilationUnitSyntax);
    }

    protected abstract void RegisterCodeFix(CodeFixContext context, Diagnostic diagnostic, TCompilationUnitSyntax compilationUnitSyntax);

    private async Task<SyntaxReference> GetDeclaringSyntaxReference(CodeFixContext context, TInvocationExpressionSyntax invocationExpression)
    {
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
        var invocationOperation = semanticModel.GetOperation(invocationExpression) as IInvocationOperation;
        var actualProxyTypeSymbol = _substituteProxyAnalysis.GetActualProxyTypeSymbol(invocationOperation);
        var syntaxReference = actualProxyTypeSymbol.DeclaringSyntaxReferences.FirstOrDefault();
        return syntaxReference;
    }

    private TCompilationUnitSyntax FindCompilationUnitSyntax(SyntaxNode syntaxNode)
    {
        return syntaxNode.Parent.Ancestors().OfType<TCompilationUnitSyntax>().LastOrDefault();
    }
}