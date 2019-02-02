using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class CallInfoAnalyzer : AbstractCallInfoAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax, ElementAccessExpressionSyntax>
    {
        private static ImmutableArray<Type> callHierarchy = ImmutableArray.Create(
            typeof(MemberAccessExpressionSyntax),
            typeof(InvocationExpressionSyntax));

        public CallInfoAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override SyntaxNode GetSubstituteCall(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            if (methodSymbol.IsExtensionMethod)
            {
                switch (methodSymbol.MethodKind)
                {
                    case MethodKind.ReducedExtension:
                        return invocationExpressionSyntax.Expression.DescendantNodes().First();
                    case MethodKind.Ordinary:
                        return invocationExpressionSyntax.ArgumentList.Arguments.First().Expression;
                    default:
                        return null;
                }
            }

            // TODO fix
            using (var descendantNodesEnumerator = invocationExpressionSyntax.DescendantNodes().GetEnumerator())
            {
                var hierarchyEnumerator = callHierarchy.GetEnumerator();
                while (hierarchyEnumerator.MoveNext() && descendantNodesEnumerator.MoveNext())
                {
                    if (descendantNodesEnumerator.Current.GetType().GetTypeInfo().IsAssignableFrom(hierarchyEnumerator.Current.GetTypeInfo()) == false)
                    {
                        return null;
                    }
                }

                if (hierarchyEnumerator.MoveNext() == false)
                {
                    var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(descendantNodesEnumerator.Current);

                    if (symbol.Symbol is IMethodSymbol mSymbol && mSymbol.ReducedFrom == null)
                    {
                        return ((InvocationExpressionSyntax)descendantNodesEnumerator.Current).ArgumentList.Arguments
                            .First().Expression;
                    }

                    return descendantNodesEnumerator.Current;
                }
            }

            return null;
        }

        protected override IEnumerable<ExpressionSyntax> GetArgumentExpressions(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.Select(arg => arg.Expression);
        }

        protected override AbstractCallInfoFinder<InvocationExpressionSyntax, ElementAccessExpressionSyntax> GetCallInfoFinder()
        {
            return new CallInfoCallFinder();
        }

        protected override SyntaxNode GetCastTypeExpression(ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            switch (indexerExpressionSyntax.Parent)
            {
                case BinaryExpressionSyntax binaryExpressionSyntax when binaryExpressionSyntax.OperatorToken.Kind() == SyntaxKind.AsKeyword:
                    return binaryExpressionSyntax.Right;
                case CastExpressionSyntax castExpressionSyntax:
                    return castExpressionSyntax.Type;
                default:
                    return null;
            }
        }

        protected override SyntaxNode GetAssignmentExpression(ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            if (indexerExpressionSyntax.Parent is AssignmentExpressionSyntax assignmentExpressionSyntax)
            {
                return assignmentExpressionSyntax.Right;
            }

            return null;
        }

        protected override ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            return syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(indexerExpressionSyntax).Symbol ??
                   syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(indexerExpressionSyntax.Expression).Symbol;
        }

        protected override int? GetArgAtPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var position = syntaxNodeAnalysisContext.SemanticModel.GetConstantValue(invocationExpressionSyntax.ArgumentList.Arguments.First().Expression);
            return (int?)(position.HasValue ? position.Value : null);
        }

        protected override int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            var position = syntaxNodeAnalysisContext.SemanticModel.GetConstantValue(indexerExpressionSyntax.ArgumentList.Arguments.First().Expression);
            return (int?)(position.HasValue ? position.Value : null);
        }

        protected override bool CanCast(Compilation compilation, ITypeSymbol sourceSymbol, ITypeSymbol destinationSymbol)
        {
            return compilation.ClassifyConversion(sourceSymbol, destinationSymbol).Exists;
        }

        protected override bool IsAssignableTo(Compilation compilation, ITypeSymbol fromSymbol, ITypeSymbol toSymbol)
        {
            var conversion = compilation.ClassifyConversion(fromSymbol, toSymbol);
            return conversion.Exists && conversion.IsExplicit == false && conversion.IsNumeric == false;
        }
    }
}