using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractNonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider : AbstractSuppressDiagnosticsCodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage);

    protected override IEnumerable<ISymbol> GetSuppressibleSymbol(SemanticModel model, SyntaxNode syntaxNode, ISymbol? symbol)
    {
        var operation = model.GetOperation(syntaxNode);
        if (operation == null)
        {
           return Enumerable.Empty<ISymbol>();
        }

        var ancestorOperation = operation.Ancestors()
            .FirstOrDefault(ancestor => AbstractNonSubstitutableMemberArgumentMatcherAnalyzer.MaybeAllowedAncestors.Contains(ancestor.Kind));

        if (ancestorOperation == null)
        {
            return Enumerable.Empty<ISymbol>();
        }

        if (ancestorOperation is IInvocationOperation invocationOperation && invocationOperation.TargetMethod.MethodKind == MethodKind.Constructor)
        {
            return Enumerable.Empty<ISymbol>();
        }

        return base.GetSuppressibleSymbol(model, ancestorOperation.Syntax, ancestorOperation.ExtractSymbol());
    }
}