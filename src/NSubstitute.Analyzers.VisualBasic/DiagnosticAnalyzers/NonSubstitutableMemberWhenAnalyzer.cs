using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal class NonSubstitutableMemberWhenAnalyzer : AbstractNonSubstitutableMemberWhenAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        public NonSubstitutableMemberWhenAnalyzer()
            : base(new DiagnosticDescriptorsProvider(), new SubstitutionNodeFinder())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
    }
}