[Leia este post em Português](https://programing-fun.blogspot.com/2023/05/uma-funcionalidade-do-cecilifier-pouco.html)

[Lire cet post en français.](https://programing-fun.blogspot.com/2023/05/little-unknown-cecilifier-feature.html)

1. [Structs in C# are fun](https://programing-fun.blogspot.com/2023/06/structs-in-c-are-fun-part-19.html)
1. [Brief introduction to Value Types vs Reference Types](https://programing-fun.blogspot.com/2023/07/structs-in-c-are-fun-part-29-brief.html)
1. [Field initialization in structs](https://programing-fun.blogspot.com/2023/08/structs-in-c-are-fun-part-39-field.html)
1. [Constructors and struct behavior](https://programing-fun.blogspot.com/2023/11/structs-in-c-are-fun-part-49.html)
1. [Other scenarios in which struct constructors behavior may surprise you](https://programing-fun.blogspot.com/2023/12/structs-in-c-are-fun-part-59-other.html).
1. Struct with default argument values in constructors, a.k.a, are you not confused yet?(_this post_)
1. `required` feature from C# 11 will not save your ~~a**~~ job.
1. Struct used as default argument values.
1. Bonus: Struct evolution in C#.

In the [previous post](https://programing-fun.blogspot.com/2023/12/structs-in-c-are-fun-part-59-other.html) we saw that the C# compiler may not emit a ctor invocation in some scenarios. Unfortunately the experience with constructors in structs, at least in my opinion, may get even more confusing.

To illustrate my point, suppose you have the following code:

```CSharp #:1
Print(new S2()); 
Print(new S2(13));

void Print(S2 s) => System.Console.WriteLine(s.v);

struct S2
{ 
    public int v;
    public S2(int i = 42)  => v = i;
    public S2() => v = 84;
}
```

What do you expect to happen when you compile/run this code[^1]?

1. Compiler will emit an error claiming `new S2()` is an ambiguous call.
1. It compiles and outputs 42, 13.
1. It compiles and outputs 84, 13.

The correct answer, which may surprise some developers, is that it compiles successfully and prints `84` and `13` (i.e, the third option).

That happens because in line 1 the C# compiler sees the parameterless constructor as a _better match_ than the one with the default parameter value ruling out options 1 and 2[^1]. The bright side is that even not being totally obvious, at least this behavior is consistent across classes/structs.

However, with structs, it may get even more complex/confusing; imagine you have exactly the same code as above, with the only difference being the removal of the parameterless constructor:

```CSharp #
Print(new S2()); 
Print(new S2(13));

void Print(S2 s) => System.Console.WriteLine(s.v);

struct S2
{ 
    public int v;
    public S2(int i = 42)  => v = i;
}
```

now, for sure it will print 42, 13, right ?

Nope! In this scenario, differently from classes, a constructor with default values for all of its parameters is not invoked, even on an *explicit* new expressions[^2] which means the code above will print `0` and `13`[^4].

In the next post we'll take a quick look into [required members](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-11.0/required-members) C# 11 feature[^3] as a way to help pin-pointing scenarios in which no constructor is being invoked.

As always, all feedback is welcome.

Have fun!

[^1]: To be fair the behavior here is the same for classes, so at least it is consistent.

[^2]: The specification of parameterless constructors in structs explicitly [states that this behavior is the same as in previous versions of C#](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/parameterless-struct-constructors#new).

[^3]: I am aware that this is not the intended use of this feature but it may be used to flag struct instantiations that does not invoke a constructor.

[^4]: That happens because line #1 is equivalent to `Print(default(S2))`; for more details see [this post](https://programing-fun.blogspot.com/2023/11/structs-in-c-are-fun-part-49.html).