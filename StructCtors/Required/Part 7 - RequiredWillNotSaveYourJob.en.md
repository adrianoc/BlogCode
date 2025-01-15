[Leia este post em Português](https://programing-fun.blogspot.com/2023/05/uma-funcionalidade-do-cecilifier-pouco.html)

[Lire cet post en français.](https://programing-fun.blogspot.com/2023/05/little-unknown-cecilifier-feature.html)

1. [Structs in C# are fun](https://programing-fun.blogspot.com/2023/06/structs-in-c-are-fun-part-19.html)
1. [Brief introduction to Value Types vs Reference Types](https://programing-fun.blogspot.com/2023/07/structs-in-c-are-fun-part-29-brief.html)
1. [Field initialization in structs](https://programing-fun.blogspot.com/2023/08/structs-in-c-are-fun-part-39-field.html)
1. [Constructors and struct behavior](https://programing-fun.blogspot.com/2023/11/structs-in-c-are-fun-part-49.html)
1. [Other scenarios in which struct constructors behavior may surprise you](https://programing-fun.blogspot.com/2023/12/structs-in-c-are-fun-part-59-other.html).
1. [Struct with default argument values in constructors, a.k.a, are you not confused yet?](https://programing-fun.blogspot.com/2024/01/structs-in-c-are-fun-part-69-struct.html).
1. `required` feature from C# 11 will not save your ~~a**~~ job. (_this post_)
1. Struct used as default argument values.
1. Conclusion.

In the previous posts of this series, while exploring the behavior of constructors in structs in C#, we've learned a couple of scenarios in which such constructors are not invoked by the compiler (despite the syntax leading us to believe the contrary).

The history of the current post is a little bit embarrassing; during the process of defining the topics I'd cover I've learned about a C# 11 feature called [`required members`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required) which I naively though could be used to flag these scenarios so I planned to add a post showing how to achieve that; whowever during investigation/drafting the post, I've realized that that was not one of the goals of this feature and that there were multiple corner cases in which no warning would be emitted even though no constructor would be invoked[^1].

The main idea would be to to mark all members (field/properties) that would be initialized in the constructors as `required` and add the [`SetsRequiredMembers`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.requiredmemberattribute?view=net-8.0) attribute to these constructors in such a way that, due to the non-initialization of the fields, the compiler would emit a warning/error in cases in which no constructor was invoked.

This thecnique works well if one wants to catch one of the most problematic cases: value types with no _parameterless constructor_ and at least one constructor in which all of its parameters are optional. To exemplify this, lets take our last example from the previous post and modify it as described in the _main idea_ section above:

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

With this change in place, instead of silently getting an instance of `S2` initialized with zero (as opposed to the expected 42 if a constructor were to be invoked), we get the folowing error:

>error CS9035: Required member 'S2.v' must be set in the object initializer or attribute constructor.

Not perfect, given that the message will probably be very confusing if one is (incorrectly) expecting `S2` constructor to be invoked but there are other limitations that renders this approach non viable:

  - Failure to detect constructors not being invoked (at least) in _default expression_ and _array instantiations_.
  - Even in scenarios in which this would work it is impossible to garantee that a ctor will be invoked (for instance, if we change the code at line #1 to `new S2() { v = 5 }`, the constructor will still not be invoked but no warning/error will be emitted)

With that in mind I think that writing a custom Roslyn Analyzer would be the best bet to verify, at compile time, that a struct constructor is being invoked but I am not sure how feasible it would be to ensure that flow analysis captures all scenarios and don't produce too many false positive/negatives.

As always, all feedback is welcome.

Have fun!

[^1]: After realising that I've changed the title of the post :).

---
[Test code](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwGIATGAMwEs8YACAVQGUBRAfQCVmBFegSU4AiAWABQAAQAMtMQEYAdAMoBDAOZ4IAZwyUwGuQGEIZAIJ4lAGwCeGyhoDcosQCYZo0QDclUWrACOtAF5aMnIlHHMMAAp2GF8ASjtaAHok2lcRT28/J0DaGgB3WhjfSITkpIBvWgA1Cxw6IIBWWgBfWkSU2icPLx9YgGZcgqLYyJkynqzYlCGYQuKxtC6JkVEU2QBOSL85WvN6hMcZLezduphD8WPtgbP9i4crk+m7g8e3ES1cMAwR31EKqIiJRyAwWBxuHxBLQYbRRLDpIM/DhKLASLRqL89vVHkQYOYNHRYfDYWJBpiaudcTA8CQQSSYQzaABtRgwDAaYootEAWRgAFtgDAoBoALpMsl/SIUyhLCkAKzigQAfJT7rl5YliSIEUyUqz2ZzYtyYCQ+YLhWK9UlJQslSq1fVcignI9dTrYQaOVzUabzUKReKPTDbaMKQhck4JPaAqrsQ1aAhHi0Ps5aAIKACJYMM+RpXhfpQlUzAcGEdJrgAiXO0H7QWsWcymyuXBEpkQtIA===)