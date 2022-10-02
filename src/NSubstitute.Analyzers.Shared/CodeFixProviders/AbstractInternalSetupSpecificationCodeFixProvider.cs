using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractInternalSetupSpecificationCodeFixProvider<TCompilationUnitSyntax> : CodeFixProvider
    where TCompilationUnitSyntax : SyntaxNode
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.InternalSetupSpecification);

    protected abstract string ReplaceModifierCodeFixTitle { get; }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var invocationExpression = root.FindNode(context.Span, getInnermostNodeForTie: true);
        var syntaxReference = await GetDeclaringSyntaxReference(context, invocationExpression);

        if (syntaxReference == null)
        {
            return;
        }

        var syntaxNode = await syntaxReference.GetSyntaxAsync();

        if (syntaxNode == null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create("Add protected modifier", token => AddModifierRefactoring(context.Document, syntaxNode, Accessibility.Protected, token)),
            context.Diagnostics);
        context.RegisterCodeFix(
            CodeAction.Create(ReplaceModifierCodeFixTitle, token => ReplaceModifierRefactoring(context.Document, syntaxNode, Accessibility.Internal, Accessibility.Public, token)),
            context.Diagnostics);

        var compilationUnitSyntax = FindCompilationUnitSyntax(syntaxNode);

        if (compilationUnitSyntax == null)
        {
            return;
        }

        RegisterAddInternalsVisibleToAttributeCodeFix(context, compilationUnitSyntax);
    }

    protected abstract Task<Document> AddModifierRefactoring(
        Document document,
        SyntaxNode node,
        Accessibility accessibility,
        CancellationToken cancellationToken);

    protected abstract Task<Document> ReplaceModifierRefactoring(
        Document document,
        SyntaxNode node,
        Accessibility fromAccessibility,
        Accessibility toAccessibility,
        CancellationToken cancellationToken);

    protected abstract void RegisterAddInternalsVisibleToAttributeCodeFix(CodeFixContext context, TCompilationUnitSyntax compilationUnitSyntax);

    private async Task<SyntaxReference> GetDeclaringSyntaxReference(CodeFixContext context, SyntaxNode invocationExpression)
    {
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
        var symbol = semanticModel.GetSymbolInfo(invocationExpression).Symbol;

        var firstOrDefault = symbol.DeclaringSyntaxReferences.FirstOrDefault();

        return firstOrDefault;
    }

    private TCompilationUnitSyntax FindCompilationUnitSyntax(SyntaxNode syntaxNode)
    {
        return syntaxNode.Parent.Ancestors().OfType<TCompilationUnitSyntax>().LastOrDefault();
    }
}