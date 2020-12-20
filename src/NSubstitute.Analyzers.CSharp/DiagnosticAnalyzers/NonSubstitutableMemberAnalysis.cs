using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class NonSubstitutableMemberAnalysis : AbstractNonSubstitutableMemberAnalysis
    {
        public static NonSubstitutableMemberAnalysis Instance { get; } = new NonSubstitutableMemberAnalysis();

        private NonSubstitutableMemberAnalysis()
        {
        }

        protected override ImmutableHashSet<Type> KnownNonVirtualSyntaxKinds { get; } = ImmutableHashSet.Create(
            typeof(LiteralExpressionSyntax));
    }
}