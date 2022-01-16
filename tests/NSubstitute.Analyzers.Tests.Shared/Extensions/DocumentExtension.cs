using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace NSubstitute.Analyzers.Tests.Shared.Extensions;

public static class DocumentExtension
{
    public static async Task<string> ToFullString(
        this Document document,
        bool simplify = false,
        bool format = false,
        CancellationToken cancellationToken = default)
    {
        if (simplify)
        {
            document = await Simplifier.ReduceAsync(document, Simplifier.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (format)
        {
            root = Formatter.Format(root, Formatter.Annotation, document.Project.Solution.Workspace);
        }

        return root.ToFullString();
    }

    public static async Task<Document> ApplyCodeAction(this Document document, CodeAction codeAction)
    {
        var operations = await codeAction.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);

        return operations
            .OfType<ApplyChangesOperation>()
            .Single()
            .ChangedSolution
            .GetDocument(document.Id);
    }
}