using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstitute.Analyzers.CSharp.Extensions
{
    internal static class ExpressionSyntaxExtensions
    {
        public static IList<ExpressionSyntax> GetParameterExpressionsFromArrayArgument(this ExpressionSyntax expression)
        {
            InitializerExpressionSyntax initializer;
            switch (expression.Kind())
            {
                case SyntaxKind.ArrayCreationExpression:
                    initializer = ((ArrayCreationExpressionSyntax)expression).Initializer;
                    break;
                case SyntaxKind.ImplicitArrayCreationExpression:
                    initializer = ((ImplicitArrayCreationExpressionSyntax)expression).Initializer;
                    break;
                default:
                    return null;
            }

            return initializer.Expressions.ToList();
        }
    }
}