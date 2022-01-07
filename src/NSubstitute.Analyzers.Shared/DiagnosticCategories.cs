namespace NSubstitute.Analyzers.Shared;

internal enum DiagnosticCategory
{
    [DisplayName("Non-substitutable member")]
    NonVirtualSubstitution = 1,

    [DisplayName("Substitute creation")]
    SubstituteCreation,

    [DisplayName("Argument specification")]
    ArgumentSpecification,

    [DisplayName("Call configuration")]
    CallConfiguration,

    [DisplayName("Usage")]
    Usage
}