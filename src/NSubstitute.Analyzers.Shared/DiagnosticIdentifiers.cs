namespace NSubstitute.Analyzers.Shared
{
    internal class DiagnosticIdentifiers
    {
        public static readonly string NonVirtualSetupSpecification = "NS001";
        public static readonly string UnusedReceived = "NS002";
        public static readonly string SubstituteForPartsOfUsedForInterface = "NS003";
        public static readonly string SubstituteForWithoutAccessibleConstructor = "NS004";
        public static readonly string SubstituteForConstructorParametersMismatch = "NS005";
        public static readonly string SubstituteForInternalMember = "NS006";
        public static readonly string SubstituteConstructorMismatch = "NS007";
        public static readonly string SubstituteMultipleClasses = "NS008";
        public static readonly string SubstituteConstructorArgumentsForInterface = "NS009";
        public static readonly string SubstituteConstructorArgumentsForDelegate = "NS010";
        public static readonly string NonVirtualReceivedSetupSpecification = "NS010";
    }
}