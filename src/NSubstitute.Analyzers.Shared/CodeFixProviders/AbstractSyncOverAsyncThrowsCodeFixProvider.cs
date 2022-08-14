using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractSyncOverAsyncThrowsCodeFixProvider<TInvocationExpressionSyntax> : CodeFixProvider
    where TInvocationExpressionSyntax : SyntaxNode
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIdentifiers.SyncOverAsyncThrows);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic =
            context.Diagnostics.FirstOrDefault(diag => diag.Descriptor.Id == DiagnosticIdentifiers.SyncOverAsyncThrows);

        if (diagnostic == null)
        {
            return;
        }

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        if (!(root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is
                TInvocationExpressionSyntax invocation))
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
        var methodSymbol = (IMethodSymbol)semanticModel.GetSymbolInfo(invocation).Symbol;
        var supportsThrowsAsync = SupportsThrowsAsync(semanticModel.Compilation);

        if (!supportsThrowsAsync && methodSymbol.Parameters.Any(param => param.Type.IsCallInfoDelegate(semanticModel)))
        {
            return;
        }

        var replacementMethod = GetReplacementMethodName(methodSymbol, useModernSyntax: supportsThrowsAsync);

        var codeAction = CodeAction.Create(
            $"Replace with {replacementMethod}",
            ct => CreateChangedDocument(context, semanticModel, invocation, methodSymbol, supportsThrowsAsync, ct),
            nameof(AbstractSyncOverAsyncThrowsCodeFixProvider<TInvocationExpressionSyntax>));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    protected abstract SyntaxNode GetExpression(TInvocationExpressionSyntax invocationExpressionSyntax);

    protected abstract SyntaxNode UpdateMemberExpression(TInvocationExpressionSyntax invocationExpressionSyntax, SyntaxNode updatedNameSyntax);

    private async Task<Document> CreateChangedDocument(
        CodeFixContext context,
        SemanticModel semanticModel,
        TInvocationExpressionSyntax currentInvocationExpression,
        IMethodSymbol invocationSymbol,
        bool useModernSyntax,
        CancellationToken cancellationToken)
    {
        var documentEditor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);
        var invocationOperation = (IInvocationOperation)semanticModel.GetOperation(currentInvocationExpression);

        var updatedInvocationExpression = useModernSyntax
            ? await CreateThrowsAsyncInvocationExpression(
                currentInvocationExpression,
                invocationSymbol,
                context)
            : await CreateReturnInvocationExpression(
                currentInvocationExpression,
                invocationOperation,
                invocationSymbol,
                context);

        documentEditor.ReplaceNode(currentInvocationExpression, updatedInvocationExpression);

        return documentEditor.GetChangedDocument();
    }

    private async Task<SyntaxNode> CreateThrowsAsyncInvocationExpression(
        TInvocationExpressionSyntax currentInvocationExpression,
        IMethodSymbol invocationSymbol,
        CodeFixContext context)
    {
        var updatedMethodName =
            invocationSymbol.IsThrowsSyncMethod()
                ? MetadataNames.NSubstituteThrowsAsyncMethod
                : MetadataNames.NSubstituteThrowsAsyncForAnyArgsMethod;

        var documentEditor = await DocumentEditor.CreateAsync(context.Document);
        var syntaxGenerator = documentEditor.Generator;

        var nameSyntax = invocationSymbol.IsGenericMethod
            ? syntaxGenerator.GenericName(updatedMethodName, invocationSymbol.TypeArguments)
            : syntaxGenerator.IdentifierName(updatedMethodName);

        return UpdateMemberExpression(currentInvocationExpression, nameSyntax);
    }

    private async Task<SyntaxNode> CreateReturnInvocationExpression(
        TInvocationExpressionSyntax currentInvocationExpression,
        IInvocationOperation invocationOperation,
        IMethodSymbol invocationSymbol,
        CodeFixContext context)
    {
        var documentEditor = await DocumentEditor.CreateAsync(context.Document);
        var syntaxGenerator = documentEditor.Generator;

        var fromExceptionInvocationExpression =
            CreateFromExceptionInvocationExpression(syntaxGenerator, invocationOperation);

        var returnsMethodName =
            invocationSymbol.IsThrowsSyncMethod() ? "Returns" : "ReturnsForAnyArgs";

        if (invocationSymbol.MethodKind == MethodKind.Ordinary)
        {
            return CreateReturnOrdinalInvocationExpression(
                currentInvocationExpression,
                invocationOperation,
                syntaxGenerator,
                fromExceptionInvocationExpression,
                returnsMethodName);
        }

        return CreateReturnExtensionInvocationExpression(
            currentInvocationExpression,
            syntaxGenerator,
            fromExceptionInvocationExpression,
            returnsMethodName);
    }

    private static SyntaxNode CreateReturnOrdinalInvocationExpression(
        TInvocationExpressionSyntax currentInvocationExpression,
        IInvocationOperation invocationOperation,
        SyntaxGenerator syntaxGenerator,
        SyntaxNode fromExceptionInvocationExpression,
        string returnsMethodName)
    {
        return syntaxGenerator.InvocationExpression(
            syntaxGenerator.MemberAccessExpression(
                syntaxGenerator.DottedName("NSubstitute.SubstituteExtensions"), returnsMethodName),
            invocationOperation.Arguments.First(arg => arg.Parameter.Ordinal == 0).Value.Syntax,
            fromExceptionInvocationExpression).WithTriviaFrom(currentInvocationExpression);
    }

    private SyntaxNode CreateReturnExtensionInvocationExpression(
        TInvocationExpressionSyntax currentInvocationExpression,
        SyntaxGenerator syntaxGenerator,
        SyntaxNode fromExceptionInvocationExpression,
        string returnsMethodName)
    {
        var expressionSyntax = GetExpression(currentInvocationExpression);

        var accessExpression =
            syntaxGenerator.MemberAccessExpression(expressionSyntax, returnsMethodName);

        return syntaxGenerator.InvocationExpression(accessExpression, fromExceptionInvocationExpression)
            .WithTriviaFrom(currentInvocationExpression);
    }

    private static SyntaxNode CreateFromExceptionInvocationExpression(
        SyntaxGenerator syntaxGenerator,
        IInvocationOperation invocationOperation)
    {
        var argumentSyntax = GetExceptionCreationExpression(invocationOperation, syntaxGenerator);

        var fromExceptionInvocationExpression =
            syntaxGenerator.InvocationExpression(
                syntaxGenerator.MemberAccessExpression(
                    syntaxGenerator.DottedName("System.Threading.Tasks.Task"), "FromException"),
                argumentSyntax);
        return fromExceptionInvocationExpression;
    }

    private static SyntaxNode GetExceptionCreationExpression(
        IInvocationOperation invocationOperation,
        SyntaxGenerator syntaxGenerator)
    {
        if (invocationOperation.TargetMethod.IsGenericMethod)
        {
            return syntaxGenerator.ObjectCreationExpression(
                syntaxGenerator.TypeExpression(invocationOperation.TargetMethod.TypeArguments.First()));
        }

        if (invocationOperation.Instance != null)
        {
            return invocationOperation.Arguments.First().Value.Syntax;
        }

        return invocationOperation.Arguments.OrderBy(arg => arg.Parameter.Ordinal)
            .First(arg => arg.Parameter.Ordinal > 0).Value.Syntax;
    }

    private static bool SupportsThrowsAsync(Compilation compilation)
    {
        var exceptionExtensionsTypeSymbol = compilation.GetTypeByMetadataName("NSubstitute.ExceptionExtensions.ExceptionExtensions");

        return exceptionExtensionsTypeSymbol != null &&
               exceptionExtensionsTypeSymbol.GetMembers(MetadataNames.NSubstituteThrowsAsyncMethod).IsEmpty == false;
    }

    private static string GetReplacementMethodName(IMethodSymbol methodSymbol, bool useModernSyntax)
    {
        if (useModernSyntax)
        {
            return methodSymbol.IsThrowsSyncMethod()
                ? MetadataNames.NSubstituteThrowsAsyncMethod
                : MetadataNames.NSubstituteThrowsAsyncForAnyArgsMethod;
        }

        return methodSymbol.IsThrowsSyncMethod()
            ? MetadataNames.NSubstituteReturnsMethod
            : MetadataNames.NSubstituteReturnsForAnyArgsMethod;
    }
}