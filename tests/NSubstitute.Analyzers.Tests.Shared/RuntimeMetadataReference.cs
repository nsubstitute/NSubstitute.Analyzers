using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.Shared
{
    public class RuntimeMetadataReference
    {
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);

        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        private static readonly MetadataReference TasksExtensionsReference = MetadataReference.CreateFromFile(Assembly.Load("System.Threading.Tasks.Extensions").Location);

        private static readonly MetadataReference NetStandard = MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location);

        private static readonly MetadataReference LinqExpressionReference = MetadataReference.CreateFromFile(typeof(Expression).Assembly.Location);

        static RuntimeMetadataReference()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var systemRuntimeReference = GetAssemblyMetadataReference(assemblies, "System.Runtime");
            var systemThreadingTasksReference = GetAssemblyMetadataReference(assemblies, "System.Threading.Tasks");

            Default = ImmutableArray.Create(
                CorlibReference,
                SystemCoreReference,
                CodeAnalysisReference,
                NSubstituteLatestReference,
                NetStandard,
                TasksExtensionsReference,
                LinqExpressionReference,
                systemRuntimeReference,
                systemThreadingTasksReference);
        }

        public static ImmutableArray<MetadataReference> Default { get; }

        public static MetadataReference NSubstitute422Reference { get; } =
            MetadataReference.CreateFromFile("nsubstitute-4.2.2/NSubstitute.dll");

        public static MetadataReference NSubstituteLatestReference { get; } =
            MetadataReference.CreateFromFile("nsubstitute-latest/NSubstitute.dll");

        private static MetadataReference GetAssemblyMetadataReference(IReadOnlyList<Assembly> assemblies, string name)
        {
            return MetadataReference.CreateFromFile(assemblies.First(n => n.GetName().Name == name).Location);
        }
    }
}