``` ini

BenchmarkDotNet=v0.10.1, OS=Linux
Processor=?, ProcessorCount=8
Frequency=1000000000 Hz, Resolution=1.0000 ns, Timer=UNKNOWN
dotnet cli version=2.2.203
  [Host] : .NET Core 4.6.27514.02, 64bit RyuJITDEBUG

Allocated=N/A  

```
                                         Method |  Job | Runtime | Mean |
----------------------------------------------- |----- |-------- |----- |
 RunNonSubstitutableMemberWhenAnalyzerBenchmark |  Clr |     Clr |   NA |
 RunNonSubstitutableMemberWhenAnalyzerBenchmark | Core |    Core |   NA |

Benchmarks with issues:
  NonSubstitutableMemberWhenAnalyzerBenchmark.RunNonSubstitutableMemberWhenAnalyzerBenchmark: Clr(Runtime=Clr)
  NonSubstitutableMemberWhenAnalyzerBenchmark.RunNonSubstitutableMemberWhenAnalyzerBenchmark: Core(Runtime=Core)
