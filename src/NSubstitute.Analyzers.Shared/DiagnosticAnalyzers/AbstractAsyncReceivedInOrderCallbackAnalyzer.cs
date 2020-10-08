using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractAsyncReceivedInOrderCallbackAnalyzer<TSyntaxKind> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
    {
        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        protected AbstractAsyncReceivedInOrderCallbackAnalyzer(
            IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            _analyzeInvocationAction = AnalyzeInvocation;
            SupportedDiagnostics = ImmutableArray.Create(diagnosticDescriptorsProvider.AsyncCallbackUsedInReceivedInOrder);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected abstract int AsyncExpressionRawKind { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected abstract IEnumerable<SyntaxToken?> GetCallbackArgumentSyntaxTokens(SyntaxNode node);

        protected sealed override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            if (!(syntaxNodeContext.SemanticModel.GetOperation(syntaxNodeContext.Node) is IInvocationOperation invocationOperation))
            {
                return;
            }

            if (invocationOperation.TargetMethod.IsReceivedInOrderMethod() == false)
            {
               return;
            }

            foreach (var argument in invocationOperation.Arguments)
            {
                var asyncToken = GetCallbackArgumentSyntaxTokens(argument.Value.Syntax)
                    .FirstOrDefault(token => token.HasValue && token.Value.RawKind == AsyncExpressionRawKind);

                if (asyncToken.HasValue == false)
                {
                   continue;
                }

                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.AsyncCallbackUsedInReceivedInOrder,
                    asyncToken.Value.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }
    }
}