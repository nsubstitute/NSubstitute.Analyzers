using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.Shared.Fixtures
{
    public class AnalyzersConventionFixture
    {
        public void AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining<T>(string expectedLanguage)
        {
            AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining(typeof(T), expectedLanguage);
        }

        public void AssertDiagnosticAnalyzerInheritanceFromAssemblyContaining<T>()
        {
            AssertDiagnosticAnalyzerInheritanceFromAssemblyContaining(typeof(T));
        }

        public void AssertDiagnosticAnalyzerInheritanceFromAssemblyContaining(Type type)
        {
            var diagnosticAnalyzerTypes = type.Assembly.GetTypesAssignableTo<DiagnosticAnalyzer>().ToList();
            var abstractDiagnosticAnalyzerTypes = type.Assembly.GetTypesAssignableTo<AbstractDiagnosticAnalyzer>().ToList();

            abstractDiagnosticAnalyzerTypes.Should().BeEquivalentTo(diagnosticAnalyzerTypes);
        }

        public void AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining<T>(string expectedLanguage)
        {
            AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining(typeof(T), expectedLanguage);
        }

        public void AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining(Type type, string expectedLanguage)
        {
            var types = type.Assembly.GetTypesAssignableTo<CodeFixProvider>().ToList();

            types.Should().OnlyContain(innerType => innerType.GetCustomAttributes<ExportCodeFixProviderAttribute>(false).Count() == 1, "because each code fix provider should be marked with only one attribute ExportCodeFixProviderAttribute");
            types.SelectMany(innerType => innerType.GetCustomAttributes<ExportCodeFixProviderAttribute>(false)).Should()
                .OnlyContain(
                    attr => attr.Languages.Length == 1 && attr.Languages.Count(lang => lang == expectedLanguage) == 1,
                    $"because each code fix provider should support only selected language ${expectedLanguage}");
        }

        public void AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining(Type type, string expectedLanguage)
        {
            var types = type.Assembly.GetTypesAssignableTo<DiagnosticAnalyzer>().ToList();

            types.Should().OnlyContain(innerType => innerType.GetCustomAttributes<DiagnosticAnalyzerAttribute>(false).Count() == 1, "because each analyzer should be marked with only one attribute DiagnosticAnalyzerAttribute");
            types.SelectMany(innerType => innerType.GetCustomAttributes<DiagnosticAnalyzerAttribute>(false)).Should()
                .OnlyContain(
                    attr => attr.Languages.Length == 1 && attr.Languages.Count(lang => lang == expectedLanguage) == 1,
                    $"because each analyzer should support only selected language ${expectedLanguage}");
        }
    }
}