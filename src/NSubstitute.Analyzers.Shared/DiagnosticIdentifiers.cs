namespace NSubstitute.Analyzers.Shared
{
    internal class DiagnosticIdentifiers
    {
        public const string NonVirtualSetupSpecification = "NS1001";
        public const string UnusedReceived = "NS1002";
        public const string PartialSubstituteForUnsupportedType = "NS1003";
        public const string SubstituteForWithoutAccessibleConstructor = "NS1004";
        public const string SubstituteForConstructorParametersMismatch = "NS1005";
        public const string SubstituteForInternalMember = "NS1006";
        public const string SubstituteConstructorMismatch = "NS1007";
        public const string SubstituteMultipleClasses = "NS1008";
        public const string SubstituteConstructorArgumentsForInterface = "NS1009";
        public const string SubstituteConstructorArgumentsForDelegate = "NS1010";
        public const string NonVirtualReceivedSetupSpecification = "NS1011";
        public const string NonVirtualWhenSetupSpecification = "NS1012";
        public const string ReEntrantSubstituteCall = "NS1013";
        public const string CallInfoArgumentOutOfRange = "NS1014";
        public const string CallInfoCouldNotConvertParameterAtPosition = "NS1015";
        public const string CallInfoCouldNotFindArgumentToThisCall = "NS1016";
        public const string CallInfoMoreThanOneArgumentOfType = "NS1017";
        public const string CallInfoArgumentSetWithIncompatibleValue = "NS1018";
        public const string CallInfoArgumentIsNotOutOrRef = "NS1019";
    }
}