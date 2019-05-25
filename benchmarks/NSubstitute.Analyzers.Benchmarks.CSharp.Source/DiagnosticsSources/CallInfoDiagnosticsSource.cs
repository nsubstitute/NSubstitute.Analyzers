using NSubstitute.Analyzers.Benchmarks.Shared.Models;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources
{
    public class CallInfoDiagnosticsSource
    {
        public void NS3000_UnableToFindMatchingArgument()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>())
                .Returns(callInfo =>
                {
                    callInfo.ArgAt<int>(10);
                    return null;
                });

            SubstituteExtensions.Returns(
                substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>()),
                callInfo =>
                {
                    callInfo.ArgAt<int>(10);
                    return null;
                });
        }

        public void NS3001_CouldNotConvertParameter()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>())
                .Returns(callInfo =>
                {
                    _ = (decimal)callInfo[1];
                    _ = (decimal)callInfo.Args()[1];
                    callInfo.ArgAt<decimal>(1);
                    return null;
                });

            SubstituteExtensions.Returns(substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>()), callInfo =>
            {
                _ = (decimal)callInfo[1];
                _ = (decimal)callInfo.Args()[1];
                callInfo.ArgAt<decimal>(1);
                return null;
            });
        }

        public void NS3002_CouldNotFindArgumentOfTypeToThisCall()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.ObjectReturningMethod().Returns(callInfo => callInfo.Arg<object>());

            SubstituteExtensions.Returns(substitute.ObjectReturningMethod(), callInfo => callInfo.Arg<object>());
        }

        public void NS3003_ThereIsMoreThanOneArgumentOfGivenTypeToThisCall()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>())
                .Returns(callInfo =>
                {
                    callInfo.Arg<int>();
                    return null;
                });

            SubstituteExtensions.Returns(
                substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>()),
                callInfo =>
                {
                    callInfo.Arg<int>();
                    return null;
                });
        }

        public void NS3004_CouldNotSetValueOfTypeToArgumentBecauseTypesAreIncompatible()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.ObjectReturningMethodWithRefArguments(ref Arg.Any<int>(), ref Arg.Any<int>(), ref Arg.Any<decimal>())
                .Returns(callInfo => callInfo[0] = "invalid");
            SubstituteExtensions.Returns(
                substitute.ObjectReturningMethodWithRefArguments(ref Arg.Any<int>(), ref Arg.Any<int>(), ref Arg.Any<decimal>()),
                callInfo => callInfo[0] = "invalid");
        }

        public void NS3005_CouldNotSetArgumentAsItIsNotRefOrOutArgument()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>())
                .Returns(callInfo => callInfo[0] = 1);

            SubstituteExtensions.Returns(
                substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>()),
                callInfo => callInfo[0] = 1);
        }

        public void NS3006_ConflictingArgumentAssignments()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.ObjectReturningMethodWithRefArguments(ref Arg.Any<int>(), ref Arg.Any<int>(), ref Arg.Any<decimal>())
                .Returns(callInfo => callInfo[0] = 1)
                .AndDoes(callInfo => callInfo[0] = 2);

            SubstituteExtensions.Returns(
                    substitute.ObjectReturningMethodWithRefArguments(ref Arg.Any<int>(), ref Arg.Any<int>(), ref Arg.Any<decimal>()),
                    callInfo => callInfo[0] = 1)
                .AndDoes(callInfo => callInfo[0] = 2);
        }
    }
}