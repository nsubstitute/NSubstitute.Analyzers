using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonSubstitutableMemberArgumentMatcherAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct, Enum
    {
        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        private readonly INonSubstitutableMemberAnalysis _nonSubstitutableMemberAnalysis;

        private readonly int _invocationExpressionRawKind;

        private readonly int[] _parentInvocationSyntaxNodeHierarchy;

        protected abstract ImmutableHashSet<int> MaybeAllowedArgMatcherAncestors { get; }

        protected abstract ImmutableHashSet<int> IgnoredArgMatcherAncestors { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractNonSubstitutableMemberArgumentMatcherAnalyzer(
            INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis,
            IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            _nonSubstitutableMemberAnalysis = nonSubstitutableMemberAnalysis;
            _analyzeInvocationAction = AnalyzeInvocation;
            SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage);
            _invocationExpressionRawKind = (int)Convert.ChangeType(InvocationExpressionKind, typeof(int));
            _parentInvocationSyntaxNodeHierarchy = new[] { _invocationExpressionRawKind };
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

            if (!(methodSymbolInfo.Symbol is IMethodSymbol methodSymbol))
            {
                return;
            }

            if (methodSymbol.IsArgMatcherLikeMethod() == false)
            {
                return;
            }

            AnalyzeArgLikeMethod(syntaxNodeContext, invocationExpression, methodSymbol);
        }

        private void AnalyzeArgLikeMethod(
            SyntaxNodeAnalysisContext syntaxNodeContext,
            TInvocationExpressionSyntax argInvocationExpression,
            IMethodSymbol invocationExpressionSymbol)
        {
            var enclosingExpression = FindMaybeAllowedEnclosingExpression(argInvocationExpression);

            // if Arg is used with not allowed expression, find if it is used in ignored ones eg. var x = Arg.Any
            // as variable might be used later on
            if (enclosingExpression == null)
            {
                var ignoredEnclosingExpression = FindIgnoredEnclosingExpression(argInvocationExpression);

                if (ignoredEnclosingExpression == null)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage,
                        argInvocationExpression.GetLocation());

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                    return;
                }
            }

            if (enclosingExpression == null)
            {
                return;
            }

            var operation = syntaxNodeContext.SemanticModel.GetOperation(enclosingExpression);

            if (operation.IsEventAssignmentOperation())
            {
                return;
            }

            var memberReferenceOperation = GetMemberReferenceOperation(operation);

            if (AnalyzeEnclosingExpression(
                syntaxNodeContext,
                argInvocationExpression,
                enclosingExpression,
                memberReferenceOperation))
            {
                return;
            }

            AnalyzeAssignment(
                syntaxNodeContext,
                argInvocationExpression,
                invocationExpressionSymbol,
                memberReferenceOperation);
        }

        private bool AnalyzeEnclosingExpression(
            SyntaxNodeAnalysisContext syntaxNodeContext,
            TInvocationExpressionSyntax argInvocationExpression,
            SyntaxNode enclosingExpression,
            IMemberReferenceOperation memberReferenceOperation)
        {
            var enclosingExpressionSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(enclosingExpression);
            var enclosingExpressionSymbol = memberReferenceOperation?.Member ??
                                            enclosingExpressionSymbolInfo.Symbol;

            if (enclosingExpressionSymbol == null)
            {
                return AnalyzeEnclosingExpressionCandidateSymbols(
                    syntaxNodeContext,
                    argInvocationExpression,
                    enclosingExpression,
                    enclosingExpressionSymbolInfo);
            }

            if (_nonSubstitutableMemberAnalysis.Analyze(
                syntaxNodeContext,
                enclosingExpression,
                enclosingExpressionSymbol).CanBeSubstituted != false)
            {
                return false;
            }

            syntaxNodeContext.TryReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage,
                    argInvocationExpression.GetLocation()),
                enclosingExpressionSymbol);

            return true;
        }

        private bool AnalyzeEnclosingExpressionCandidateSymbols(
            SyntaxNodeAnalysisContext syntaxNodeContext,
            TInvocationExpressionSyntax argInvocationExpression,
            SyntaxNode enclosingExpression,
            SymbolInfo enclosingExpressionSymbolInfo)
        {
            if (enclosingExpressionSymbolInfo.CandidateSymbols.Length == 0)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage,
                    argInvocationExpression.GetLocation());

                syntaxNodeContext.TryReportDiagnostic(diagnostic, null);
                return true;
            }

            foreach (var candidateSymbol in enclosingExpressionSymbolInfo.CandidateSymbols)
            {
                if (_nonSubstitutableMemberAnalysis.Analyze(
                    syntaxNodeContext,
                    enclosingExpression,
                    candidateSymbol).CanBeSubstituted == false)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage,
                        argInvocationExpression.GetLocation());

                    syntaxNodeContext.TryReportDiagnostic(diagnostic, candidateSymbol);
                    return true;
                }
            }

            return false;
        }

        private void AnalyzeAssignment(
            SyntaxNodeAnalysisContext syntaxNodeContext,
            TInvocationExpressionSyntax argInvocationExpression,
            IMethodSymbol argInvocationExpressionSymbol,
            IMemberReferenceOperation memberReferenceOperation)
        {
            if (memberReferenceOperation == null)
            {
               return;
            }

            var syntaxNode = memberReferenceOperation.Syntax;

            if (IsWithinWhenLikeMethod(syntaxNodeContext, syntaxNode))
            {
                return;
            }

            if (argInvocationExpressionSymbol.IsArgDoLikeMethod())
            {
               return;
            }

            if (IsPrecededByReceivedLikeMethod(syntaxNodeContext, syntaxNode))
            {
                return;
            }

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage,
                argInvocationExpression.GetLocation());

            syntaxNodeContext.TryReportDiagnostic(diagnostic, memberReferenceOperation.Member);
        }

        private bool IsPrecededByReceivedLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntaxNode)
        {
            var parentInvocationSyntaxNode = syntaxNode.GetParentNode(_parentInvocationSyntaxNodeHierarchy);
            return parentInvocationSyntaxNode != null &&
                   syntaxNodeContext.SemanticModel.GetSymbolInfo(parentInvocationSyntaxNode).Symbol.IsReceivedLikeMethod();
        }

        private bool IsWithinWhenLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntaxNode)
        {
            var invocation = syntaxNode.Ancestors().FirstOrDefault(ancestor => ancestor.RawKind == _invocationExpressionRawKind);

            return invocation != null && syntaxNodeContext.SemanticModel.GetSymbolInfo(invocation).Symbol.IsWhenLikeMethod();
        }

        private static IMemberReferenceOperation GetMemberReferenceOperation(IOperation operation)
        {
            switch (operation)
            {
                case IAssignmentOperation assignmentOperation
                    when assignmentOperation.Target is IMemberReferenceOperation memberReferenceOperation:
                    return memberReferenceOperation;
                case IBinaryOperation binaryOperation
                    when binaryOperation.LeftOperand is IMemberReferenceOperation binaryMemberReferenceOperation:
                    return binaryMemberReferenceOperation;
                case IBinaryOperation binaryOperation
                    when binaryOperation.LeftOperand is IConversionOperation conversionOperation &&
                         conversionOperation.Operand is IMemberReferenceOperation conversionMemberReference:
                    return conversionMemberReference;
                case IExpressionStatementOperation expressionStatementOperation
                    when
                    expressionStatementOperation.Operation is ISimpleAssignmentOperation simpleAssignmentOperation &&
                    simpleAssignmentOperation.Target is IMemberReferenceOperation memberReferenceOperation:
                    return memberReferenceOperation;
                default:
                    return null;
            }
        }

        private SyntaxNode FindMaybeAllowedEnclosingExpression(TInvocationExpressionSyntax invocationExpression) =>
            FindEnclosingExpression(invocationExpression, MaybeAllowedArgMatcherAncestors);

        private SyntaxNode FindIgnoredEnclosingExpression(TInvocationExpressionSyntax invocationExpressionSyntax) =>
            FindEnclosingExpression(invocationExpressionSyntax, IgnoredArgMatcherAncestors);

        private static SyntaxNode FindEnclosingExpression(TInvocationExpressionSyntax invocationExpression, ImmutableHashSet<int> ancestors)
        {
            return invocationExpression.Ancestors()
                .FirstOrDefault(ancestor => ancestors.Contains(ancestor.RawKind));
        }
    }
}