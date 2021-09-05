using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal abstract class AbstractNonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider : AbstractSuppressDiagnosticsCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage);

        protected abstract ImmutableHashSet<int> MaybeAllowedArgMatcherAncestors { get; }

        protected override IEnumerable<ISymbol> GetSuppressibleSymbol(SemanticModel model, SyntaxNode syntaxNode, ISymbol symbol)
        {
            var ancestorNode = syntaxNode.Ancestors()
                .FirstOrDefault(ancestor => MaybeAllowedArgMatcherAncestors.Contains(ancestor.RawKind));

            if (ancestorNode == null)
            {
                return Enumerable.Empty<ISymbol>();
            }

            if (model.GetSymbolInfo(ancestorNode).Symbol is IMethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.Constructor)
            {
                return Enumerable.Empty<ISymbol>();
            }

            return base.GetSuppressibleSymbol(model, ancestorNode, model.GetSymbolInfo(ancestorNode).Symbol);
        }
    }
}