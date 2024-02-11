### 1.0.17 (11 February 2024)
- [#213](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/213) - NS1004 false positive when using argument matcher in a separate method
- [#212](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/212) - Disable CallInfo analysis when Arg.AnyType used for argument matching
- [#207](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/207) - Detect non virtual received checks for event subscription
- [#202](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/202) - NS5000 false positive when checking if an event subscription was received
- [#197](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/197) - Add .netstandard2.0 target to NSubstitute.Analyzers.CSharp

### 1.0.16 (22 January 2023)

- [#186](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/186) - Support for NSubstitute 4.4 API
- [#181](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/181) - Introducing code fix provider for NS5001
- [#174](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/174) - NS5004, detecting usage of arg matchers in `WithAnyArgs` like methods
- [#164](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/164) - NS5003, detecting synchronous exceptions thrown from async methods
- [#153](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/153) - Handling out of order parameters

### 1.0.15 (13 November 2021)

- [#172](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/172) - NS1004 false positive when using arg matchers with properties
- [#171](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/171) - NS1004 false positive when using arg matchers with properties
- [#163](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/163) - NS1004 false positive when using arg matchers with ternary operator
- [#162](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/162) - NS2002, NS2004 false positive when using Substitute.For, Substitute.ForPartsOf with constructor with params
- [#160](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/160) - NS1004 false positive when using arg matchers with properties for When calls
- [#159](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/159) - NS1004 false positive when using Arg.Do with properties
- [#157](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/157) - Handling generic CallInfo&lt;T&gt; for incoming NSubstitute releases
- [#152](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/152) - Marking analyzers as development dependency
- [#150](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/150) - NS3002 false positive when using arg matchers with nested returns

### 1.0.14 (10 November 2020)

- [#147](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/147) - NS1004 false positive when subscribing to event
- [#144](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/144) - NS2001 false positive for internal class with protected internal constructor
- [#108](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/108) - Detect non-virtual calls in Received.InOrder

### 1.0.13 (8 April 2020)

- [#142](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/142) - Simplify InternalVisibilityTo when System.Runtime.CompilerServices in scope
- [#138](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/138) - Quick-action for introducing substitute in constructor argument
- [#135](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/135) - Detect usages of Received like methods in Received.InOrder block
- [#134](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/134) - Detect async callback in Received.InOrder

### 1.0.12 (5 February 2020)

- [#131](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/131) - NS5000 is not reported for ReceivedExtensions.Received calls 
- [#130](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/130) - NS1001 is not reported for ReceivedExtensions.Received calls 
- [#128](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/128) - Code fix provider for NS400 
- [#126](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/126) - Extend NS400 to detect reentrant property substitution 
- [#102](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/102) - Produce Strong Named Signed Assemblies  
- [#35](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/35) - Detect Arg matcher misuse 

### 1.0.11 (15 October 2019)

- [#124](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/124) - NS4000 false positive when using an initializer
- [#118](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/118) - NS3002 false positive
- [#115](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/115) - Eliminate unnecessary allocations and improve performance
- [#106](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/106) - NS4000 false positive in foreach loop when element used twice

### 1.0.10 (5 June 2019)

 - [#103](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/103) - Enable concurrent execution for every analyzer
 - [#93](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/93) - NS4000 Unexpected warning in foreach loop
 
### 1.0.9 (12 April 2019)

 - [#91](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/91) - Erroneous NS1002 errors for Arg.Any calls

### 1.0.8 (08 April 2019)

 - [#89](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/89) - NS1002 doesn't detect non-virtual calls from base class
 - [#87](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/87) - NS1002 highlights wrong member
 - [#78](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/78) - Update documentation so as it includes information about min supported Visual Studio version
 - [#76](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/76) - Analyze callInfo usages for Do method
 - [#62](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/62) - Detecting conflicting out/ref arguments

### 1.0.7 (15 March 2019)

 - [#79](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/79) - NS2003 triggers a warning when InternalsVisibleTo using strong name
 - [#73](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/73) - NS1001 highlights wrong member
 - [#70](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/70) - Implement code fix providers for substituting for internal member
 - [#66](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/66) - Detect substituting for `internal` members without `InternalsVisibleTo`

### 1.0.6 (21 February 2019)

 - [#71](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/71) - Incorrect NS4000 when using typeof expression

### 1.0.5 (09 February 2019)

 - [#61](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/61) - CallInfo analysis for AndDoes method

### 1.0.4 (03 January 2019)

 - [#57](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/57) - Incorrect NS3004 for derived types
 - [#56](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/56) - Analyzer 'NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers.ReEntrantSetupAnalyzer' threw an exception of type 'System.ArgumentException' with message 'SyntaxTree is not part of the compilation'

### 1.0.3 (22 November 2018)

 - [#54](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/54) - Rule NS2003 does not detect InternalsVisibleTo correctly

### 1.0.2 (07 November 2018)

 - [#52](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/52) - Detect substitution for non-virtual members for ReturnsNull like methods

### 1.0.1 (29 October 2018)

 - [#50](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/50) - Enforce release notes update during package publish
 - [#47](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/47) - Over-enthusiastic NS4000 error (Returns within Returns)
 - [#46](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/46) - ArgumentException on ReEntrantSetupAnalyzer on first load

### 1.0.0 (23 October 2018)

 - [#42](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/42) - Add basic documentation

### 0.1.0-beta6 (07 October 2018)

 - [#40](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/40) - Group diagnostics into categories

### 0.1.0-beta5 (30 September 2018)

 - [#38](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/38) - Add code fix provider for exposing internal types to proxy generator

### 0.1.0-beta4 (23 September 2018)

 - [#36](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/36) - ReEntrantSetupAnalyzer - syntax node is not within syntax tree

### 0.1.0-beta3 (07 September 2018)

 - [#31](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/31) - Update nuget package with proper NSubstitute.Analyzers icon
 - [#30](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/30) - Detect wrong usages of callInfo argument access

### 0.1.0-beta2 (01 August 2018)

 - [#28](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/28) - Detect non-virtual setup for Throws and ThrowsForAnyArgs
 - [#26](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/26) - Checking constructor arguments passed via `SubstitutionContext.Current.SubstituteFactory.Create`/`SubstitutionContext.Current.SubstituteFactory.CreatePartial`
 - [#22](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/22) - Unify resource files
 - [#18](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/18) - Detect non-virtual When...Do calls
 - [#16](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/16) - Detect non-virtual received calls
 - [#12](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/12) - Detect re-entrant substitute calls
 - [#11](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/11) - Allow configuration of safe API misuses
 - [#1](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/1) - Checking constructor arguments passed via `Substitute.For`/`Substitute.ForPartsOf`

### 0.1.0-beta1 (01 August 2018)
 - Initial release