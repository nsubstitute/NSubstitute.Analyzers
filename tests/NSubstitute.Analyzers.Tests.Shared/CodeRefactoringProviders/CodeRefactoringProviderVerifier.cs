using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;

public abstract class CodeRefactoringProviderVerifier : CodeVerifier
{
    protected CodeRefactoringProviderVerifier(WorkspaceFactory workspaceFactory)
        : base(workspaceFactory)
    {
    }

    protected abstract CodeRefactoringProvider CodeRefactoringProvider { get; }

    protected async Task VerifyRefactoring(string oldSource, string newSource, int? refactoringIndex = null)
    {
        var parserResult = TextParser.GetSpans(oldSource);
        var spans = parserResult.Spans;
        if (spans.Length == 0)
        {
            throw new ArgumentException("Refactoring spans should not be empty", nameof(oldSource));
        }

        using var workspace = new AdhocWorkspace();
        var project = AddProject(workspace.CurrentSolution, parserResult.Text);
        var document = project.Documents.Single();

        var actions = await RegisterCodeRefactoringActions(document, spans.Single().Span);

        var codeAction = actions[refactoringIndex ?? 0];
        var updatedDocument = await document.ApplyCodeAction(codeAction);
        var updatedSource = await updatedDocument.ToFullString();
        updatedSource.Should().Be(newSource);
    }

    private async Task<ImmutableArray<CodeAction>> RegisterCodeRefactoringActions(Document document, TextSpan span)
    {
        var builder = ImmutableArray.CreateBuilder<CodeAction>();
        var context = new CodeRefactoringContext(document, span, a => builder.Add(a), CancellationToken.None);
        await CodeRefactoringProvider.ComputeRefactoringsAsync(context);

        return builder.ToImmutable();
    }
}