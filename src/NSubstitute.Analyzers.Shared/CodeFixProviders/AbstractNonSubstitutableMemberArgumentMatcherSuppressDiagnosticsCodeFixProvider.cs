using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal abstract class AbstractNonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider : AbstractSuppressDiagnosticsCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage);

        protected abstract ImmutableArray<ImmutableArray<int>> AllowedAncestorPaths { get; }

        protected override IEnumerable<ISymbol> GetSuppressibleSymbol(SemanticModel model, SyntaxNode syntaxNode, ISymbol symbol)
        {
            var ancestorNode = syntaxNode.GetAncestorNode(AllowedAncestorPaths);

            if (ancestorNode == null)
            {
                return Enumerable.Empty<ISymbol>();
            }

            return base.GetSuppressibleSymbol(model, ancestorNode, model.GetSymbolInfo(ancestorNode).Symbol);
        }
    }
}