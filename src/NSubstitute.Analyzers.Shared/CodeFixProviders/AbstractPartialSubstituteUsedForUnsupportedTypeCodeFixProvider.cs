using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider<TInvocationExpression, TGenericNameSyntax, TIdentifierNameSyntax, TNameSyntax>
    : CodeFixProvider
    where TInvocationExpression : SyntaxNode
    where TGenericNameSyntax : TNameSyntax
    where TIdentifierNameSyntax : TNameSyntax
    where TNameSyntax : SyntaxNode
{
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.PartialSubstituteForUnsupportedType);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.FirstOrDefault(diag => diag.Descriptor.Id == DiagnosticIdentifiers.PartialSubstituteForUnsupportedType);
        if (diagnostic != null)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var invocationExpression = (TInvocationExpression)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            if (semanticModel.GetSymbolInfo(invocationExpression).Symbol is not IMethodSymbol methodSymbol)
            {
                return;
            }

            var title = methodSymbol.Name == MetadataNames.SubstituteFactoryCreatePartial ? "Use SubstituteFactory.Create" : "Use Substitute.For";
            var codeAction = CodeAction.Create(
                title,
                ct => CreateChangedDocument(context, root, methodSymbol, invocationExpression),
                nameof(AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider<TInvocationExpression, TGenericNameSyntax, TIdentifierNameSyntax, TNameSyntax>));
            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }

    protected abstract TInnerNameSyntax GetNameSyntax<TInnerNameSyntax>(TInvocationExpression methodInvocationNode) where TInnerNameSyntax : TNameSyntax;

    protected abstract TInnerNameSyntax GetUpdatedNameSyntax<TInnerNameSyntax>(TInnerNameSyntax nameSyntax, string identifierName) where TInnerNameSyntax : TNameSyntax;

    private Task<Document> CreateChangedDocument(CodeFixContext context, SyntaxNode root, IMethodSymbol methodSymbol, TInvocationExpression invocationExpression)
    {
        SyntaxNode nameNode;
        SyntaxNode updateNameNode;

        if (methodSymbol.IsGenericMethod)
        {
            var genericNameSyntax = GetNameSyntax<TGenericNameSyntax>(invocationExpression);
            nameNode = genericNameSyntax;
            updateNameNode = GetUpdatedNameSyntax(genericNameSyntax, MetadataNames.NSubstituteForMethod);
        }
        else
        {
            var identifierNameSyntax = GetNameSyntax<TIdentifierNameSyntax>(invocationExpression);
            nameNode = identifierNameSyntax;
            updateNameNode = GetUpdatedNameSyntax(identifierNameSyntax, MetadataNames.SubstituteFactoryCreate);
        }

        var forNode = invocationExpression.ReplaceNode(nameNode, updateNameNode);

        var replaceNode = root.ReplaceNode(invocationExpression, forNode);

        return Task.FromResult(context.Document.WithSyntaxRoot(replaceNode));
    }
}