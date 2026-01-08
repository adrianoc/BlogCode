## Introduction

Over the years, software has grown significantly in both size and complexity. What started as relatively simple, self-contained applications has evolved into large, distributed systems with numerous external dependencies, including third-party libraries, complex runtimes, and interconnected services. This rise in complexity has inevitably led to an increase in bugs, prompting the development of more rigorous testing practices to address these challenges. One of the key advancements in this area has been the adoption of various forms of automated testing.

As a quick reminder, automated testing has multiple benefits, among others:

1. Repeatability / Consistency
1. Rise of confidence in quality
1. Reduces regressions
1. Can augment documentation
1. When paired with source control, can simplify maintaining history of bugs/fixes.
1. etc.

but, unfortunately, it also comes with potential problems/concerns[^1]:

1. Extra overhead to write / maintain the test suite,
1. Resource consumption to run the tests (when deployed in CI/CD environments),
1. Coupling between production / test code,
1. etc.

While the potential to enhance software quality is undeniable, the extent to which this potential is realized largely depends on the quality of the test suite itself. Among the many metrics we can use to assess such quality, **code coverage**[^2] - the focus of today's discussion - is, in my opinion, an important one. Before continuing, I should clarify that I’m not a testing expert by any means. However, I’ve been working in software development for the past 30 years and have actively practiced unit and integration testing for the last 20 to 25 years in personal as well professional projects. :) 

That said after finishing reading this post you should have a basic understanding on what code coverage is and why one should consider (or not) using this metric.

## Code coverage to help

Code coverage in the context of automated tests is a measurement of how much of the code under test is exercised when running the tests. In theory, the higher the coverage a code base have the smaller the chances of bugs going unnoticed.

 1. How one can use it to improve testing quality ?
 1. Not covered paths probably hides bugs (no tests)
 1. Projects should monitor the percentage of coverage accepted and have CI/CD jobs failures in case the threshold is crossed.
 1. Code coverage is not a silver bullet
- Its is another tool in the good software development practices (code review for instance)
- Achieving 100% code coverage is not always feasible  / desirable (diminishing returns)
- Tests itself hardly cover 100% of the features - for instance problem domain is too large and testing 100% would take to long (+ corner cases that are not tested)
- Even 100% coverture does not implies bug free code (this is missing one test, Do(4, 8) for instance):
```C#
int Do(int i, int j)
{
  if (i % 2 == 0)
  {
     Console.WriteLine("1");
  }

  if (j > 5)
  {
     Console.WriteLine("2");
  }
}

[Test]
void T1()
{
  Do(4, 1);
}

[Test]
void T2()
{
  Do(3, 8);
}
```

## 1. Real case examples
   1. [TypeDeclarationResolver](https://app.codecov.io/gh/adrianoc/cecilifier/commit/387b001103bd6c3284c556ead0cf34de4e45755c/blob/Cecilifier.Core/Misc/TypeDeclarationResolver.cs?dropdown=coverage) missing handling for enum members and tests for a bunch of other member types.

   1. [This commit adds tests and fixes issues](37e2926dfd46), [spotted in partially covered lines in NoCapturedVariableValidator.cs](https://app.codecov.io/gh/adrianoc/cecilifier/commit/387b001103bd6c3284c556ead0cf34de4e45755c/blob/Cecilifier.Core/AST/NoCapturedVariableValidator.cs?dropdown=coverage#L27)
   3. [This commit adds tests and fixes for a couple of types derived from TypeSyntax](39f921d4bb42730c28e753b03f1a74bdcd0e3f1c) which has been spotted [in  TypeSyntaxExtensions](https://app.codecov.io/gh/adrianoc/cecilifier/commit/387b001103bd6c3284c556ead0cf34de4e45755c/blob/Cecilifier.Core/Extensions/TypeSyntaxExtensions.cs?dropdown=coverage#L13)
   4. #330 4611269fe89310b63f087edfbe829000ceea4221 Adds disabled test for assignment of inline array stored in a field to a Span<T> (#320) - 

## 1. Configuring coverage collection in .NET
1. Local
2. CI

Create project and tests

```bash
dotnet new classlib -n mylib
dotnet new classlib -n mylibtest
dotnet add mylibtest/mylibtest.csproj reference mylib/mylib.csproj
```

Add required packages in to be able to run tests...
```bash
dotnet add mylibtest/mylibtest.csproj package NUnit 
dotnet add mylibtest/mylibtest.csproj package NUnit3TestAdapter 
dotnet add mylibtest/mylibtest.csproj package Microsoft.NET.Test.Sdk
```

Add code coverage collection/report generation support
In this example I am using [AltCover](https://github.com/SteveGilham/altcover) but there are many other options which may vary in the [FIX WORDING: features here is odd]features/quality/reliability of the output reports; also if you plan to track of coverage evolution or have you may want to use .....

```bash
dotnet add mylibtest/mylibtest.csproj package AltCover
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Run tests and generate a report
```bash
dotnet test -p:AltCover=true
reportgenerator -reports:coverage.xml -targetdir:coveragereport
```

Alternatively you can install AltCover visualizer also
```bash
dotnet tool install --global altcover.visualizer
```

[Add a video showing a simple test project - or maybe even cecilifier ?]

## 1. Resources
 - https://dev.to/bigboybamo/beginners-guide-to-test-coverage-with-nunit-coverlet-and-reportgenerator-3fde
 - https://github.com/danielpalme/ReportGenerator
 - [CRAP code](https://testing.googleblog.com/2011/02/this-code-is-crap.html).

---- 

[^1] When correctly applied it is expected that this overhead will be offseted by the gains brought by the usage of such automated tests (after all there's no point in using any technique if the coast of applying it outweighs the benefits it brings).

[^2] Note that there are lots of controversy regarding the effectiveness of code coverage in improving code quality; many believe that this metric can even be harmful; to be honest I haven't used it so extensively to have a strong opinion; my usages of it were mostly to pinpoint areas of the code that are missing coverage which in general is a good indicator of areas that may have hidden bugs.

---

- What is code coverage
- Approach:
  - Find usage of not fully covered method/properties and either add tests or configure coverage software to ignore
- Real usage:
  - [SizeofPrimitiveType](https://app.codecov.io/gh/adrianoc/cecilifier/blob/vnext/Cecilifier.Core%2FExtensions%2FTypeExtensions.cs?trend=all%20time#L63)
      - Most likely we don't have tests covering some use case involving arrays.
      - For sure, no coverage for Doubles, System.Single (float)
  - [StindOpCodeFor](https://app.codecov.io/gh/adrianoc/cecilifier/blob/vnext/Cecilifier.Core%2FExtensions%2FTypeExtensions.cs?trend=all%20time#L93)
      - Is this even a valid scenario? stind for an array element?
