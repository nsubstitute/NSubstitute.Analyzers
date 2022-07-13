using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

internal sealed class NonSubstitutableMemberAnalysis : AbstractNonSubstitutableMemberAnalysis
{
    public static NonSubstitutableMemberAnalysis Instance { get; } = new ();

    private NonSubstitutableMemberAnalysis()
    {
    }

    protected override ImmutableHashSet<Type> KnownNonVirtualSyntaxKinds { get; } = ImmutableHashSet.Create(
        typeof(LiteralExpressionSyntax));
}