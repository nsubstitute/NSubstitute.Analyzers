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

namespace NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;

public abstract class CodeRefactoringProviderActionsVerifier : CodeVerifier
{
    protected CodeRefactoringProviderActionsVerifier(WorkspaceFactory workspaceFactory)
        : base(workspaceFactory)
    {
    }

    protected abstract CodeRefactoringProvider CodeRefactoringProvider { get; }

    protected async Task VerifyCodeActions(string source, params string[] expectedCodeActionTitles)
    {
        var parserResult = TextParser.GetSpans(source);
        var spans = parserResult.Spans.Select(position => position.Span).ToList();

        if (spans.Count == 0)
        {
            throw new ArgumentException("Refactoring spans should not be empty", nameof(source));
        }

        using (var workspace = new AdhocWorkspace())
        {
            var project = AddProject(workspace.CurrentSolution, parserResult.Text);
            var document = project.Documents.Single();

            var codeActionTasks = spans
                .Select(span => RegisterCodeRefactoringActions(document, span)).ToList();

            await Task.WhenAll(codeActionTasks);

            var codeActions = codeActionTasks.SelectMany(task => task.Result).ToList();

            codeActions.Should().NotBeNull();
            codeActions.Select(action => action.Title).Should().BeEquivalentTo(expectedCodeActionTitles ?? Array.Empty<string>());
        }
    }

    private async Task<ImmutableArray<CodeAction>> RegisterCodeRefactoringActions(Document document, TextSpan span)
    {
        var builder = ImmutableArray.CreateBuilder<CodeAction>();
        var context = new CodeRefactoringContext(document, span, a => builder.Add(a), CancellationToken.None);
        await CodeRefactoringProvider.ComputeRefactoringsAsync(context);

        return builder.ToImmutable();
    }
}