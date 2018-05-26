[1mdiff --git a/tests/NSubstitute.Analyzers.Test.CSharp/NSubstitute.Analyzers.Test.CSharp.csproj b/tests/NSubstitute.Analyzers.Test.CSharp/NSubstitute.Analyzers.Test.CSharp.csproj[m
[1mindex de9d811..e5dc96e 100644[m
[1m--- a/tests/NSubstitute.Analyzers.Test.CSharp/NSubstitute.Analyzers.Test.CSharp.csproj[m
[1m+++ b/tests/NSubstitute.Analyzers.Test.CSharp/NSubstitute.Analyzers.Test.CSharp.csproj[m
[36m@@ -18,9 +18,8 @@[m
     <Compile Include="..\NSubstitute.Analyzers.Test.Shared\AnalyzerTest.cs">[m
       <Link>AnalyzerTest.cs</Link>[m
     </Compile>[m
[31m-    <Compile Include="..\NSubstitute.Analyzers.Test.Shared\DiagnosticResult.cs">[m
[31m-      <Link>DiagnosticResult.cs</Link>[m
[31m-    </Compile>[m
[32m+[m[32m    <Compile Include="..\NSubstitute.Analyzers.Test.Shared\DiagnosticResult.cs" Link="DiagnosticResult.cs" />[m
[32m+[m[32m    <Compile Include="..\NSubstitute.Analyzers.Test.Shared\DiagnosticResultLocation.cs" Link="DiagnosticResultLocation.cs" />[m
     <Compile Include="..\NSubstitute.Analyzers.Test.Shared\NonVirtualSetupAnalyzerTestBase.cs">[m
       <Link>NonVirtualSetupAnalyzerTestBase.cs</Link>[m
     </Compile>[m
