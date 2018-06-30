using System.Reflection;
using System.Resources;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared
{
    internal class DiagnosticDescriptors<T>
    {
        public static readonly ResourceManager ResourceManager =
            new ResourceManager(
                $"{typeof(T).GetTypeInfo().Assembly.GetName().Name}.Resources",
                typeof(T).GetTypeInfo().Assembly);

        public static DiagnosticDescriptor NonVirtualSetupSpecification { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(NonVirtualSetupSpecification),
                id: DiagnosticIdentifiers.NonVirtualSetupSpecification,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor UnusedReceived { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(UnusedReceived),
                id: DiagnosticIdentifiers.UnusedReceived,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor UnusedReceivedForOrdinaryMethod { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(UnusedReceivedForOrdinaryMethod),
                id: DiagnosticIdentifiers.UnusedReceived,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteForPartsOfUsedForInterface { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteForPartsOfUsedForInterface),
                id: DiagnosticIdentifiers.SubstituteForPartsOfUsedForInterface,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteForWithoutAccessibleConstructor { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteForWithoutAccessibleConstructor),
                id: DiagnosticIdentifiers.SubstituteForWithoutAccessibleConstructor,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteForConstructorParametersMismatch { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteForConstructorParametersMismatch),
                id: DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteForInternalMember { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteForInternalMember),
                id: DiagnosticIdentifiers.SubstituteForInternalMember,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteConstructorMismatch { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteConstructorMismatch),
                id: DiagnosticIdentifiers.SubstituteConstructorMismatch,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteMultipleClasses { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteMultipleClasses),
                id: DiagnosticIdentifiers.SubstituteMultipleClasses,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteConstructorArgumentsForInterface { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteConstructorArgumentsForInterface),
                id: DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteConstructorArgumentsForDelegate { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteConstructorArgumentsForDelegate),
                id: DiagnosticIdentifiers.SubstituteConstructorArgumentsForDelegate,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor NonVirtualWhenSetupSpecification { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(NonVirtualWhenSetupSpecification),
                id: DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                category: DiagnosticCategories.Usage,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        private static DiagnosticDescriptor CreateDiagnosticDescriptor(
            string name, string id, string category, DiagnosticSeverity defaultSeverity, bool isEnabledByDefault)
        {
            var title = GetDiagnosticResourceString(name, nameof(DiagnosticDescriptor.Title));
            var messageFormat = GetDiagnosticResourceString(name, nameof(DiagnosticDescriptor.MessageFormat));
            var description = GetDiagnosticResourceString(name, nameof(DiagnosticDescriptor.Description));
            return new DiagnosticDescriptor(id, title, messageFormat, category, defaultSeverity, isEnabledByDefault, description);
        }

        private static LocalizableResourceString GetDiagnosticResourceString(string name, string propertyName)
        {
            return new LocalizableResourceString(name + propertyName, ResourceManager, typeof(T));
        }
    }
}