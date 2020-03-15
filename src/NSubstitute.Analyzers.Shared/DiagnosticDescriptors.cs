using System.Reflection;
using System.Resources;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared
{
    internal class DiagnosticDescriptors<T>
    {
        private static readonly ResourceManager SpecificResourceManager =
            new ResourceManager(
                $"{typeof(T).GetTypeInfo().Assembly.GetName().Name}.Resources",
                typeof(T).GetTypeInfo().Assembly);

        private static string helpLinkUriFormat = "https://github.com/nsubstitute/NSubstitute.Analyzers/blob/master/documentation/rules/{0}.md";

        public static DiagnosticDescriptor NonVirtualSetupSpecification { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(NonVirtualSetupSpecification),
                id: DiagnosticIdentifiers.NonVirtualSetupSpecification,
                category: DiagnosticCategory.NonVirtualSubstitution.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor InternalSetupSpecification { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(InternalSetupSpecification),
                id: DiagnosticIdentifiers.InternalSetupSpecification,
                category: DiagnosticCategory.NonVirtualSubstitution.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor NonVirtualReceivedSetupSpecification { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(NonVirtualReceivedSetupSpecification),
                id: DiagnosticIdentifiers.NonVirtualReceivedSetupSpecification,
                category: DiagnosticCategory.NonVirtualSubstitution.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor NonVirtualWhenSetupSpecification { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(NonVirtualWhenSetupSpecification),
                id: DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                category: DiagnosticCategory.NonVirtualSubstitution.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor PartialSubstituteForUnsupportedType { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(PartialSubstituteForUnsupportedType),
                id: DiagnosticIdentifiers.PartialSubstituteForUnsupportedType,
                category: DiagnosticCategory.SubstituteCreation.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteForWithoutAccessibleConstructor { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteForWithoutAccessibleConstructor),
                id: DiagnosticIdentifiers.SubstituteForWithoutAccessibleConstructor,
                category: DiagnosticCategory.SubstituteCreation.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteForConstructorParametersMismatch { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteForConstructorParametersMismatch),
                id: DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                category: DiagnosticCategory.SubstituteCreation.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteForInternalMember { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteForInternalMember),
                id: DiagnosticIdentifiers.SubstituteForInternalMember,
                category: DiagnosticCategory.SubstituteCreation.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteConstructorMismatch { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteConstructorMismatch),
                id: DiagnosticIdentifiers.SubstituteConstructorMismatch,
                category: DiagnosticCategory.SubstituteCreation.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteMultipleClasses { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteMultipleClasses),
                id: DiagnosticIdentifiers.SubstituteMultipleClasses,
                category: DiagnosticCategory.SubstituteCreation.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteConstructorArgumentsForInterface { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteConstructorArgumentsForInterface),
                id: DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface,
                category: DiagnosticCategory.SubstituteCreation.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor SubstituteConstructorArgumentsForDelegate { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(SubstituteConstructorArgumentsForDelegate),
                id: DiagnosticIdentifiers.SubstituteConstructorArgumentsForDelegate,
                category: DiagnosticCategory.SubstituteCreation.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor CallInfoArgumentOutOfRange { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(CallInfoArgumentOutOfRange),
                id: DiagnosticIdentifiers.CallInfoArgumentOutOfRange,
                category: DiagnosticCategory.ArgumentSpecification.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor CallInfoCouldNotConvertParameterAtPosition { get; } =
            CreateDiagnosticDescriptor(
            name: nameof(CallInfoCouldNotConvertParameterAtPosition),
            id: DiagnosticIdentifiers.CallInfoCouldNotConvertParameterAtPosition,
            category: DiagnosticCategory.ArgumentSpecification.GetDisplayName(),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor CallInfoCouldNotFindArgumentToThisCall { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(CallInfoCouldNotFindArgumentToThisCall),
                id: DiagnosticIdentifiers.CallInfoCouldNotFindArgumentToThisCall,
                category: DiagnosticCategory.ArgumentSpecification.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor CallInfoMoreThanOneArgumentOfType { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(CallInfoMoreThanOneArgumentOfType),
                id: DiagnosticIdentifiers.CallInfoMoreThanOneArgumentOfType,
                category: DiagnosticCategory.ArgumentSpecification.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor CallInfoArgumentSetWithIncompatibleValue { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(CallInfoArgumentSetWithIncompatibleValue),
                id: DiagnosticIdentifiers.CallInfoArgumentSetWithIncompatibleValue,
                category: DiagnosticCategory.ArgumentSpecification.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor CallInfoArgumentIsNotOutOrRef { get; } = CreateDiagnosticDescriptor(
            name: nameof(CallInfoArgumentIsNotOutOrRef),
            id: DiagnosticIdentifiers.CallInfoArgumentIsNotOutOrRef,
            category: DiagnosticCategory.ArgumentSpecification.GetDisplayName(),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor ReEntrantSubstituteCall { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(ReEntrantSubstituteCall),
                id: DiagnosticIdentifiers.ReEntrantSubstituteCall,
                category: DiagnosticCategory.CallConfiguration.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor UnusedReceived { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(UnusedReceived),
                id: DiagnosticIdentifiers.UnusedReceived,
                category: DiagnosticCategory.Usage.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor UnusedReceivedForOrdinaryMethod { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(UnusedReceivedForOrdinaryMethod),
                id: DiagnosticIdentifiers.UnusedReceived,
                category: DiagnosticCategory.Usage.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor ConflictingArgumentAssignments { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(ConflictingArgumentAssignments),
                id: DiagnosticIdentifiers.ConflictingArgumentAssignments,
                category: DiagnosticCategory.ArgumentSpecification.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor NonSubstitutableMemberArgumentMatcherUsage { get; } =
            CreateDiagnosticDescriptor(
                name: nameof(NonSubstitutableMemberArgumentMatcherUsage),
                id: DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage,
                category: DiagnosticCategory.NonVirtualSubstitution.GetDisplayName(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static DiagnosticDescriptor ReceivedUsedInReceivedInOrder { get; } = CreateDiagnosticDescriptor(
            name: nameof(ReceivedUsedInReceivedInOrder),
            id: DiagnosticIdentifiers.ReceivedUsedInReceivedInOrder,
            category: DiagnosticCategory.Usage.GetDisplayName(),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor AsyncCallbackUsedInReceivedInOrder { get; } = CreateDiagnosticDescriptor(
            name: nameof(AsyncCallbackUsedInReceivedInOrder),
            id: DiagnosticIdentifiers.AsyncCallbackUsedInReceivedInOrder,
            category: DiagnosticCategory.Usage.GetDisplayName(),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static DiagnosticDescriptor CreateDiagnosticDescriptor(
            string name, string id, string category, DiagnosticSeverity defaultSeverity, bool isEnabledByDefault)
        {
            var title = GetDiagnosticResourceString(name, nameof(DiagnosticDescriptor.Title));
            var messageFormat = GetDiagnosticResourceString(name, nameof(DiagnosticDescriptor.MessageFormat));
            var description = GetDiagnosticResourceString(name, nameof(DiagnosticDescriptor.Description));
            return new DiagnosticDescriptor(id, title, messageFormat, category, defaultSeverity, isEnabledByDefault, description, string.Format(helpLinkUriFormat, id));
        }

        private static LocalizableResourceString GetDiagnosticResourceString(string name, string propertyName)
        {
            var localizableResource = name + propertyName;
            var resourceManager = string.IsNullOrWhiteSpace(SpecificResourceManager.GetString(localizableResource)) ? SharedResourceManager.Instance : SpecificResourceManager;

            return new LocalizableResourceString(localizableResource, resourceManager, typeof(T));
        }
    }
}