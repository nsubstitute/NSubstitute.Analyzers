using System;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers
{
    public class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor DoNotCreateSubstituteForNonVirtualMembers = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.DoNotCreateSubstituteForNonVirtualMembers,
            title:              "Type name '{0}' contains lowercase letters",
            messageFormat:      "Type name '{0}' contains lowercase letters",
            category:           DiagnosticCategories.Substitutes,
            defaultSeverity:    DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description:        null);
    }
}