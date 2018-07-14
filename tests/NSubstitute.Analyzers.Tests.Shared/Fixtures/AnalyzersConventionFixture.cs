using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Tests.Shared.Fixtures
{
    public class AnalyzersConventionFixture
    {
        public void AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining<T>(string expectedLanguage)
        {
            AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining(typeof(T), expectedLanguage);
        }

        public void AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining<T>(string expectedLanguage)
        {
            AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining(typeof(T), expectedLanguage);
        }

        public void AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining(Type type, string expectedLanguage)
        {
            var types = GetTypesAssignableTo<CodeFixProvider>(type.Assembly).ToList();

            types.Should().OnlyContain(innerType => innerType.GetCustomAttributes<ExportCodeFixProviderAttribute>(true).Count() == 1, "because each code fix provider should be marked with only one attribute ExportCodeFixProviderAttribute");
            types.SelectMany(innerType => innerType.GetCustomAttributes<ExportCodeFixProviderAttribute>(true)).Should()
                .OnlyContain(
                    attr => attr.Languages.Length == 1 && attr.Languages.Count(lang => lang == expectedLanguage) == 1,
                    $"because each code fix provider should support only selected language ${expectedLanguage}");
        }

        public void AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining(Type type, string expectedLanguage)
        {
            var types = GetTypesAssignableTo<DiagnosticAnalyzer>(type.Assembly).ToList();

            types.Should().OnlyContain(innerType => innerType.GetCustomAttributes<DiagnosticAnalyzerAttribute>(true).Count() == 1, "because each analyzer should be marked with only one attribute DiagnosticAnalyzerAttribute");
            types.SelectMany(innerType => innerType.GetCustomAttributes<DiagnosticAnalyzerAttribute>(true)).Should()
                .OnlyContain(
                    attr => attr.Languages.Length == 1 && attr.Languages.Count(lang => lang == expectedLanguage) == 1,
                    $"because each analyzer should support only selected language ${expectedLanguage}");
        }

        public void AssertDiagnosticIdsDefinitionsFromAssemblyContaining<T>()
        {
            AssertDiagnosticIdsDefinitionsFromAssemblyContaining(typeof(T));
        }

        private void AssertDiagnosticIdsDefinitionsFromAssemblyContaining(Type type)
        {
            var diagnosticAnalyzers = GetTypesAssignableTo<DiagnosticAnalyzer>(type.Assembly);
            var exportedAnalyzers = diagnosticAnalyzers.Where(analyzer => analyzer.GetCustomAttributes<DiagnosticAnalyzerAttribute>().Any());

            var instances = exportedAnalyzers.Select(CreateInstance<DiagnosticAnalyzer>).ToList();

            var allDiagnostics = instances.SelectMany(instance => instance.SupportedDiagnostics.Select(diagnostic => diagnostic.Id)).ToList();
            var distinctDiagnostics = allDiagnostics.Distinct();

            distinctDiagnostics.Should().BeEquivalentTo(allDiagnostics, "because diagnostic ids should not be used across multiple analyzers");
        }

        private static T CreateInstance<T>(Type analyzer)
        {
            var args = analyzer.GetConstructors().First().GetParameters().Select(parameter => Substitute.For(new[] { parameter.ParameterType }, null)).ToArray();
            return (T)Activator.CreateInstance(analyzer, args);
        }

        private IEnumerable<Type> GetTypesAssignableTo<T>(Assembly assembly)
        {
            var type = typeof(T);
            return assembly.GetTypes().Where(innerType => type.IsAssignableFrom(innerType)).ToList();
        }
    }
}