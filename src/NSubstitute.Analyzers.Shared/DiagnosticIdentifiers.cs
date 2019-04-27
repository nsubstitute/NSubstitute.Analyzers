namespace NSubstitute.Analyzers.Shared
{
    internal class DiagnosticIdentifiers
    {
        public const string NonVirtualSetupSpecification = "NS1000";
        public const string NonVirtualReceivedSetupSpecification = "NS1001";
        public const string NonVirtualWhenSetupSpecification = "NS1002";
        public const string InternalSetupSpecification = "NS1003";

        public const string PartialSubstituteForUnsupportedType = "NS2000";
        public const string SubstituteForWithoutAccessibleConstructor = "NS2001";
        public const string SubstituteForConstructorParametersMismatch = "NS2002";
        public const string SubstituteForInternalMember = "NS2003";
        public const string SubstituteConstructorMismatch = "NS2004";
        public const string SubstituteMultipleClasses = "NS2005";
        public const string SubstituteConstructorArgumentsForInterface = "NS2006";
        public const string SubstituteConstructorArgumentsForDelegate = "NS2007";

        public const string CallInfoArgumentOutOfRange = "NS3000";
        public const string CallInfoCouldNotConvertParameterAtPosition = "NS3001";
        public const string CallInfoCouldNotFindArgumentToThisCall = "NS3002";
        public const string CallInfoMoreThanOneArgumentOfType = "NS3003";
        public const string CallInfoArgumentSetWithIncompatibleValue = "NS3004";
        public const string CallInfoArgumentIsNotOutOrRef = "NS3005";
        public const string ConflictingArgumentAssignments = "NS3006";

        public const string ReEntrantSubstituteCall = "NS4000";

        public const string UnusedReceived = "NS5000";

        public const string ArgumentMatcherUsedWithoutSpecifyingCall = "NS5001";
    }
}