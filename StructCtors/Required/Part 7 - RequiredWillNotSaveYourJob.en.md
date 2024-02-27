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
1. Bonus: Struct evolution in C#.

In the previous posts of this series while exploring the behavior of constructors in structs in C# we've learned a couple of scenarios in which such constructors are not invoked by the compiler (despite the syntax leading us to believe the contrary).

The history of the current post is a little bit embarrassing; during the process of defining the topics I'd cover in this series I've learned about a C# 11 feature called [`required members`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required) which I naively though could be used to solve the problem of uninitialized struct members and so this post entry was added to the list of topics. 

Later, after learning more about it, I realized that avoiding uninitialized members on struct instances was not one of its goals; nevertheless, I think it may be used to catch one of the most problematic cases: value types with no _parameterless constructor_ and at least one constructor in which all of its parameters are optional[^1].

To exemplify how this technique can be employed, lets take our last exemple from the previous post and add the `required` modifier:

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

With this change instead of silently getting an instanceof `S2` initialized with zero (as opposed to the expected  42), we get the folowing compiler error:

>error CS9035: Required member 'Req.Value' must be set in the object initializer or attribute constructor.

Not perfec wiven that it may be a bit hard to realise the error is being raised due to no constructors being invoked and also it does not flag other scenarios in which users may (wrongly) expect a constructor to be invoked.

With that in mind I think writing a custom Roslyn Analyzer would be the best bet to ensure that, at compile time, structs are being correctly initialized but I guess that it would be dauthing task ensuring that flow analysis captures all scenarios and don't produce too many false positive/negatives.

Are
As always, all feedback is welcome.

Have fun!

[^1]: However, the fact that no constructors will be invoked when initializing such types through a _default expression_ or when allocating _arrays_ of such types is not detected by this techinique.

---
[Test code](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwGIATGAMwEs8YACAVQGUBRAfQCVmBFegSU4AiAWABQAAQAMtMQEYAdAMoBDAOZ4IAZwyUwGuQGEIZAIJ4lAGwCeGyhoDcosQCYZo0QDclUWrACOtAF5aMnIlHHMMAAp2GF8ASjtaAHok2lcRT28/J0DaGgB3WhjfSITkpIBvWgA1Cxw6IIBWWgBfWkSU2icPLx9YgGZcgqLYyJkynqzYlCGYQuKxtC6JkVEU2QBOSL85WvN6hMcZLezduphD8WPtgbP9i4crk+m7g8e3ES1cMAwR31EKqIiJRyAwWBxuHxBLQYbRRLDpIM/DhKLASLRqL89vVHkQYOYNHRYfDYWJBpiaudcTA8CQQSSYQzaABtRgwDAaYootEAWRgAFtgDAoBoALpMsl/SIUyhLCkAKzigQAfJT7rl5YliSIEUyUqz2ZzYtyYCQ+YLhWK9UlJQslSq1fVcignI9dTrYQaOVzUabzUKReKPTDbaMKQhck4JPaAqrsQ1aAhHi0Ps5aAIKACJYMM+RpXhfpQlUzAcGEdJrgAiXO0H7QWsWcymyuXBEpkQtIA===)