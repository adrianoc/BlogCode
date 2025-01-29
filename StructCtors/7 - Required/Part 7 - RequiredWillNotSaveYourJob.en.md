[Leia este post em Português]()

[Lire cet post en français.]()

<script>
populateToc("https://raw.githubusercontent.com/adrianoc/BlogCode/refs/heads/main/StructCtors/toc.en.js", 'struct-series-toc');
</script>

<p id="struct-series-toc">

The history of the current post is a little bit embarrassing.

During the process of defining the topics I'd cover I've learned about a C# 11 feature called [`required members`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required) which I naively though could be used to flag these scenarios so I planned to add a post showing how to achieve that; however during investigation/drafting[^1] the content, I've realized that that was not one of the goals of this feature and that there were multiple corner cases in which no warning would be emitted even though no constructor would be invoked[^2].

The main idea would be to to mark all members (field/properties) that would be initialized in the constructors as `required` and add the [`SetsRequiredMembers`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.requiredmemberattribute?view=net-8.0) attribute to these constructors in such a way that in cases where no constructors (decorated with `SetRequiredMembersAttribute`) were to be invoked the compiler would emit a warning/error due to the non-initialization of such members.

This technique works relatively well if one wants to catch a very problematic scenario: instantiation of value types with a constructor in which all of its parameters are optional[^3]. To make the discussion more concrete, lets take our last example from the previous post and modify it as described above:

```CSharp #:8,10
Print(new S2());
Print(new S2(13));

void Print(S2 s) => System.Console.WriteLine(s.v);

struct S2
{ 
    public required int v;
    
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public S2(int i = 42)  => v = i;
}
```

With this change in place, instead of silently getting an instance of `S2` initialized with zero (as opposed to the expected 42 if the constructor were to be invoked), we get the following error:

>error CS9035: Required member 'S2.v' must be set in the object initializer or attribute constructor.

Not perfect, given that the message will probably be very confusing if one is (incorrectly, but understandably) expecting `S2` constructor to be invoked, but there are other limitations rendering this approach even less viable:

  - Inability to detect constructors not being invoked (at least) in _default expression_ and _array instantiations_.
  - Even in scenarios in which this would work it is impossible to guarantee that a ctor will be invoked (for instance, if we change the code at line #1 to `new S2() { v = 5 }`, no constructor will be invoked but no warning/error will be emitted either)

A more effective alternative (if you deploy your application as a managed one as opposed to AOTing it) to detect such scenarios is by explicitly asserting that struct instances have been initialized[^4] (either by a constructor or some other means) prior to accessing its members; since the code is being JITed one can implement this in a way users can control whether the check should be enforced or not and have very little (if any) performance impact when enforcing is disabled, as demonstrated below.

```CSharp
using System.Runtime.CompilerServices;

class Driver
{
    static void Main()
    {

        for(int i = 0; i < 100_000; i++)
        {
            var foo = new Foo(); // No constructor invoked... no warnings :(
            Thread.Sleep(100);
            foo.Use();
        }
    }

}

// The struct can be declared in a different assembly also. 
struct Foo
{
    // it is important for the field to be marked as `readonly`
    private static readonly bool _shouldValidate = Environment.GetEnvironmentVariable("VALIDATE_FOO") == "true";

    private int _i;
    private bool _isInitialized;

    public Foo(int v = 1) { _i = v; _isInitialized = true; }

    public void Use()
    {
        Verify();
        System.Console.WriteLine(_i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // It is important to ask the JIT to inline this method for the optimization to be applied.
    private readonly void Verify()
    {
        if (_shouldValidate)
        {
            if (!_isInitialized)
            {
                throw new Exception("Foo constructor was not invoked; this may be due an array declaration or ...");
            }
        }
    }
}
```

The code use the field `_shouldValidate` to control whether correct initialization[^4] should be enforced or not (in this case more specifically, if the constructor has been executed). Notice that this field is declared as `static readonly`; this is very important since with this in place the JIT knowns that, for a given struct instance, once initialized, the field value **will never change** so it is free to handle `_shouldValidate` as a constant and to not generate code to check it in the **if** in line #XX; moreover, in case it is evaluated to **false** the JIT can remove the whole `if` statement (whence the close to zero overhead mentioned before).

You can see this JIT *magic* in action by opening a terminal, creating a console application with the code above and running:

```bash
DOTNET_JitDisasm=Use dotnet run -c Release
```

which:

1. in `unix` like OSs, sets the environment variable `DOTNET_JitDisasm` value to `Use` and runs `dotnet run -c Release`.
1. builds the application in **release mode** (`-c Release`), which is requirement for the optimization to be applied.
1. instructs the JIT to dump the JITed assembly code for the method `Use()` by setting `DOTNET_JitDisasm` environment variable accordingly.

When executing that command line you should see zeros (0) and some assembly code being printed to the terminal multiple times; after some iterations you should be able to spot some assembly code resembling the one below (make sure to check the one that contains `Tier1` as opposed to `Tier0`):

```nasm
; Assembly listing for method Foo:Use():this (Tier1)
; Emitting BLENDED_CODE for X64 with AVX - Unix
; Tier1 code
; optimized code
; rsp based frame
; fully interruptible
; No PGO data
; 1 inlinees with PGO data; 0 single block inlinees; 0 inlinees without PGO data

G_M000_IG01:                ;; offset=0x0000
 
G_M000_IG02:                ;; offset=0x0000
       mov      edi, dword ptr [rdi]
 
G_M000_IG03:                ;; offset=0x0002
       tail.jmp [System.Console:WriteLine(int)]
 
; Total bytes of code 8
```

which is basically calling `System.Console.WriteLine(_i)` and returning, with no traces of the method `Verify()` invocation.

You can also play around with this example by running it as:

```bash
DOTNET_JitDisasm=Use VALIDATE_FOO=true dotnet run -c Release
```

in which case it will throw an exception (proving that uninitialized struct instances usage is detected)

or

```bash
DOTNET_JitDisasm=Use dotnet run -c Debug
```

in which case, no matter for how long the application runs, the assembly code generated for `Use()` will always call `Verify()` (i.e. optimization was not applied because application was built in **debug mode**)

With this approach one can be sure that no code is using `uninitialized` instances by simply running the code with the environment variable set to `true` and observing for exceptions.

As always, all feedback is welcome.

Have fun!

[^1]: [here](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwGIATGAMwEs8YACAVQGUBRAfQCVmBFegSU4AiAWABQAAQAMtMQEYAdAMoBDAOZ4IAZwyUwGuQGEIZAIJ4lAGwCeGyhoDcosQCYZo0QDclUWrACOtAF5aMnIlHHMMAAp2GF8ASjtaAHok2lcRT28/J0DaGgB3WhjfSITkpIBvWgA1Cxw6IIBWWgBfWkSU2icPLx9YgGZcgqLYyJkynqzYlCGYQuKxtC6JkVEU2QBOSL85WvN6hMcZLezduphD8WPtgbP9i4crk+m7g8e3ES1cMAwR31EKqIiJRyAwWBxuHxBLQYbRRLDpIM/DhKLASLRqL89vVHkQYOYNHRYfDYWJBpiaudcTA8CQQSSYQzaABtRgwDAaYootEAWRgAFtgDAoBoALpMsl/SIUyhLCkAKzigQAfJT7rl5YliSIEUyUqz2ZzYtyYCQ+YLhWK9UlJQslSq1fVcignI9dTrYQaOVzUabzUKReKPTDbaMKQhck4JPaAqrsQ1aAhHi0Ps5aAIKACJYMM+RpXhfpQlUzAcGEdJrgAiXO0H7QWsWcymyuXBEpkQtIA===) you can find some test code I used while exploring this topic.

[^2]: After realizing that I've changed the post's title :).

[^3]: This particular case is problematic due to the expectation that the behavior would match the behavior for **classes**

[^4]: Note of clarification: From the perspective of the runtime, structs are guaranteed to be initialized (by _zeroing out the whole struct_) before being used. _Initialization_ in the context of this series of posts means that all struct fields/properties have been assigned meaningful values leaving the instance in a consistent state.