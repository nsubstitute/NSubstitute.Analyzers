using System.Collections.Generic;

namespace NSubstitute.Analyzers.Shared;

internal class MetadataNames
{
    public const string NSubstituteAssemblyName = "NSubstitute";
    public const string NSubstituteArgFullTypeName = "NSubstitute.Arg";
    public const string NSubstituteArgCompatFullTypeName = "NSubstitute.Arg.Compat";
    public const string NSubstituteSubstituteExtensionsFullTypeName = "NSubstitute.SubstituteExtensions";
    public const string NSubstituteReceivedExtensionsFullTypeName = "NSubstitute.ReceivedExtensions.ReceivedExtensions";
    public const string NSubstituteReturnsExtensionsFullTypeName = "NSubstitute.ReturnsExtensions.ReturnsExtensions";
    public const string NSubstituteExceptionExtensionsFullTypeName = "NSubstitute.ExceptionExtensions.ExceptionExtensions";
    public const string NSubstituteCallInfoFullTypeName = "NSubstitute.Core.CallInfo";
    public const string NSubstituteConfiguredCallFullTypeName = "NSubstitute.Core.ConfiguredCall";
    public const string NSubstituteSubstituteFullTypeName = "NSubstitute.Substitute";
    public const string NSubstituteFactoryFullTypeName = "NSubstitute.Core.ISubstituteFactory";
    public const string NSubstituteReturnsMethod = "Returns";
    public const string NSubstituteReturnsForAnyArgsMethod = "ReturnsForAnyArgs";
    public const string NSubstituteThrowsMethod = "Throws";
    public const string NSubstituteThrowsAsyncMethod = "ThrowsAsync";
    public const string NSubstituteThrowsForAnyArgsMethod = "ThrowsForAnyArgs";
    public const string NSubstituteThrowsAsyncForAnyArgsMethod = "ThrowsAsyncForAnyArgs";
    public const string NSubstituteAndDoesMethod = "AndDoes";
    public const string NSubstituteReturnsNullMethod = "ReturnsNull";
    public const string NSubstituteReturnsNullForAnyArgsMethod = "ReturnsNullForAnyArgs";
    public const string NSubstituteDoMethod = "Do";
    public const string NSubstituteReceivedMethod = "Received";
    public const string NSubstituteReceivedWithAnyArgsMethod = "ReceivedWithAnyArgs";
    public const string NSubstituteDidNotReceiveMethod = "DidNotReceive";
    public const string NSubstituteDidNotReceiveWithAnyArgsMethod = "DidNotReceiveWithAnyArgs";
    public const string NSubstituteInOrderMethod = "InOrder";
    public const string NSubstituteReceivedFullTypeName = "NSubstitute.Received";
    public const string NSubstituteForMethod = "For";
    public const string NSubstituteForPartsOfMethod = "ForPartsOf";
    public const string SubstituteFactoryCreate = "Create";
    public const string SubstituteFactoryCreatePartial = "CreatePartial";
    public const string InternalsVisibleToAttributeFullTypeName = "System.Runtime.CompilerServices.InternalsVisibleToAttribute";
    public const string CastleDynamicProxyGenAssembly2Name = "DynamicProxyGenAssembly2";
    public const string NSubstituteWhenMethod = "When";
    public const string NSubstituteWhenForAnyArgsMethod = "WhenForAnyArgs";
    public const string NSubstituteWhenCalledType = "WhenCalled";
    public const string CallInfoArgAtMethod = "ArgAt";
    public const string CallInfoArgMethod = "Arg";
    public const string CallInfoArgTypesMethod = "ArgTypes";
    public const string ArgIsMethodName = "Is";
    public const string ArgAnyMethodName = "Any";
    public const string ArgDoMethodName = "Do";
    public const string ArgInvokeMethodName = "Invoke";
    public const string ArgInvokeDelegateMethodName = "InvokeDelegate";

    public static readonly IReadOnlyDictionary<string, string> ReturnsMethodNames = new Dictionary<string, string>
    {
        [NSubstituteReturnsMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteReturnsForAnyArgsMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteReturnsNullMethod] = NSubstituteReturnsExtensionsFullTypeName,
        [NSubstituteReturnsNullForAnyArgsMethod] = NSubstituteReturnsExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ReturnsForAnyArgsMethodNames = new Dictionary<string, string>
    {
        [NSubstituteReturnsForAnyArgsMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteReturnsNullForAnyArgsMethod] = NSubstituteReturnsExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ThrowsMethodNames = new Dictionary<string, string>
    {
        [NSubstituteThrowsMethod] = NSubstituteExceptionExtensionsFullTypeName,
        [NSubstituteThrowsAsyncMethod] = NSubstituteExceptionExtensionsFullTypeName,
        [NSubstituteThrowsForAnyArgsMethod] = NSubstituteExceptionExtensionsFullTypeName,
        [NSubstituteThrowsAsyncForAnyArgsMethod] = NSubstituteExceptionExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ThrowsForAnyArgsMethodNames = new Dictionary<string, string>
    {
        [NSubstituteThrowsForAnyArgsMethod] = NSubstituteExceptionExtensionsFullTypeName,
        [NSubstituteThrowsAsyncForAnyArgsMethod] = NSubstituteExceptionExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ThrowsSyncMethodNames = new Dictionary<string, string>
    {
        [NSubstituteThrowsMethod] = NSubstituteExceptionExtensionsFullTypeName,
        [NSubstituteThrowsForAnyArgsMethod] = NSubstituteExceptionExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ReceivedMethodNames = new Dictionary<string, string>
    {
        [NSubstituteReceivedMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteReceivedWithAnyArgsMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteDidNotReceiveMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteDidNotReceiveWithAnyArgsMethod] = NSubstituteSubstituteExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ReceivedWithQuantityMethodNames = new Dictionary<string, string>
    {
        [NSubstituteReceivedMethod] = NSubstituteReceivedExtensionsFullTypeName,
        [NSubstituteReceivedWithAnyArgsMethod] = NSubstituteReceivedExtensionsFullTypeName,
    };

    public static readonly IReadOnlyDictionary<string, string> ReceivedWithAnyArgsMethodNames = new Dictionary<string, string>
    {
        [NSubstituteReceivedWithAnyArgsMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteDidNotReceiveWithAnyArgsMethod] = NSubstituteSubstituteExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ReceivedWithAnyArgsQuantityMethodNames = new Dictionary<string, string>
    {
        [NSubstituteReceivedWithAnyArgsMethod] = NSubstituteReceivedExtensionsFullTypeName,
        [NSubstituteDidNotReceiveWithAnyArgsMethod] = NSubstituteReceivedExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> WhenMethodNames = new Dictionary<string, string>
    {
        [NSubstituteWhenMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteWhenForAnyArgsMethod] = NSubstituteSubstituteExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> WhenForAnyArgsMethodNames = new Dictionary<string, string>
    {
        [NSubstituteWhenForAnyArgsMethod] = NSubstituteSubstituteExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ArgMatchersMethodNames = new Dictionary<string, string>
    {
        [ArgIsMethodName] = NSubstituteArgFullTypeName,
        [ArgAnyMethodName] = NSubstituteArgFullTypeName,
        [ArgDoMethodName] = NSubstituteArgFullTypeName,
        [ArgInvokeMethodName] = NSubstituteArgFullTypeName,
        [ArgInvokeDelegateMethodName] = NSubstituteArgFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ArgMatchersCompatMethodNames = new Dictionary<string, string>
    {
        [ArgIsMethodName] = NSubstituteArgCompatFullTypeName,
        [ArgAnyMethodName] = NSubstituteArgCompatFullTypeName,
        [ArgDoMethodName] = NSubstituteArgCompatFullTypeName,
        [ArgInvokeMethodName] = NSubstituteArgCompatFullTypeName,
        [ArgInvokeDelegateMethodName] = NSubstituteArgCompatFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ArgMatchersIncompatibleWithForAnyArgsMethodNames = new Dictionary<string, string>
    {
        [ArgIsMethodName] = NSubstituteArgFullTypeName,
        [ArgDoMethodName] = NSubstituteArgFullTypeName,
        [ArgInvokeMethodName] = NSubstituteArgFullTypeName,
        [ArgInvokeDelegateMethodName] = NSubstituteArgFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> ArgMatchersCompatIncompatibleWithForAnyArgsMethodNames = new Dictionary<string, string>
    {
        [ArgIsMethodName] = NSubstituteArgCompatFullTypeName,
        [ArgDoMethodName] = NSubstituteArgCompatFullTypeName,
        [ArgInvokeMethodName] = NSubstituteArgCompatFullTypeName,
        [ArgInvokeDelegateMethodName] = NSubstituteArgCompatFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> InitialReEntryMethodNames = new Dictionary<string, string>
    {
        [NSubstituteReturnsMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteReturnsForAnyArgsMethod] = NSubstituteSubstituteExtensionsFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> CreateSubstituteMethodNames = new Dictionary<string, string>
    {
        [NSubstituteForMethod] = NSubstituteSubstituteFullTypeName,
        [NSubstituteForPartsOfMethod] = NSubstituteSubstituteFullTypeName,
        [SubstituteFactoryCreate] = NSubstituteFactoryFullTypeName,
        [SubstituteFactoryCreatePartial] = NSubstituteFactoryFullTypeName
    };

    public static readonly IReadOnlyDictionary<string, string> SupportingCallInfoMethodNames = new Dictionary<string, string>
    {
        [NSubstituteReturnsMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteReturnsForAnyArgsMethod] = NSubstituteSubstituteExtensionsFullTypeName,
        [NSubstituteThrowsMethod] = NSubstituteExceptionExtensionsFullTypeName,
        [NSubstituteThrowsAsyncMethod] = NSubstituteExceptionExtensionsFullTypeName,
        [NSubstituteThrowsForAnyArgsMethod] = NSubstituteExceptionExtensionsFullTypeName,
        [NSubstituteThrowsAsyncForAnyArgsMethod] = NSubstituteExceptionExtensionsFullTypeName,
        [NSubstituteAndDoesMethod] = NSubstituteConfiguredCallFullTypeName,
        [NSubstituteDoMethod] = NSubstituteWhenCalledType
    };
}