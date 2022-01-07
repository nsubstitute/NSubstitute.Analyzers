using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class DocumentExtensions
{
    public static async Task<Document> ReplaceNodeAsync(
        this Document document,
        SyntaxNode oldNode,
        SyntaxNode newNode,
        CancellationToken cancellationToken = default)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);

        var newRoot = root.ReplaceNode(oldNode, newNode);

        return document.WithSyntaxRoot(newRoot);
    }
}