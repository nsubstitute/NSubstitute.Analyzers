using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
#if CSHARP
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;
#endif
using Microsoft.CodeAnalysis.Diagnostics;
#if VISUAL_BASIC
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

namespace NSubstitute.Analyzers.Extensions
{
    public static class ExpressionSyntaxExtensions
    {
        private static readonly IList<ExpressionSyntax> EmptyExpressionList = new ExpressionSyntax[0];

        public static IList<ExpressionSyntax> GetParameterExpressionsFromArrayArgument(this ExpressionSyntax expression)
        {
#if CSHARP

            InitializerExpressionSyntax initializer = null;
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

            if (initializer == null)
            {
                return EmptyExpressionList;
            }

            return initializer.Expressions.ToList();
#elif VISUAL_BASIC

            return EmptyExpressionList;
#endif
        }
    }
}