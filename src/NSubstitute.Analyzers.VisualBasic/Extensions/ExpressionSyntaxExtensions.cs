using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace NSubstitute.Analyzers.VisualBasic.Extensions
{
    internal static class ExpressionSyntaxExtensions
    {
        public static IList<ExpressionSyntax> GetParameterExpressionsFromArrayArgument(this ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.ArrayCreationExpression:
                    var initializer = ((ArrayCreationExpressionSyntax)expression).Initializer;
                    return initializer.Initializers.ToList();
                case SyntaxKind.CollectionInitializer:
                    return ((CollectionInitializerSyntax)expression).Initializers.ToList();
                default:
                    return null;
            }
        }
    }
}