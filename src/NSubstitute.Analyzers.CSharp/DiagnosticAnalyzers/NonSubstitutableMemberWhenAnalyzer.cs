using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class NonSubstitutableMemberWhenAnalyzer : AbstractNonSubstitutableMemberWhenAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        public NonSubstitutableMemberWhenAnalyzer()
            : base(new DiagnosticDescriptorsProvider(), new SubstitutionNodeFinder())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
    }
}