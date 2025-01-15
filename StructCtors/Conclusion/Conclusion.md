[Leia este post em Português](https://programing-fun.blogspot.com/2023/05/uma-funcionalidade-do-cecilifier-pouco.html)

[Lire cet post en français.](https://programing-fun.blogspot.com/2023/05/little-unknown-cecilifier-feature.html)

1. [Structs in C# are fun](https://programing-fun.blogspot.com/2023/06/structs-in-c-are-fun-part-19.html)
1. [Brief introduction to Value Types vs Reference Types](https://programing-fun.blogspot.com/2023/07/structs-in-c-are-fun-part-29-brief.html)
1. [Field initialization in structs](https://programing-fun.blogspot.com/2023/08/structs-in-c-are-fun-part-39-field.html)
1. [Constructors and struct behavior](https://programing-fun.blogspot.com/2023/11/structs-in-c-are-fun-part-49.html)
1. [Other scenarios in which struct constructors behavior may surprise you](https://programing-fun.blogspot.com/2023/12/structs-in-c-are-fun-part-59-other.html).
1. [Struct with default argument values in constructors, a.k.a, are you not confused yet?](https://programing-fun.blogspot.com/2024/01/structs-in-c-are-fun-part-69-struct.html).
1. `required` feature from C# 11 will not save your ~~a**~~ job.
1. Struct used as default argument values.
1. Conclusion. (_this post_)

In the previous posts of this series while exploring the behavior of constructors in structs in C# we've learned a couple of scenarios in which such constructors are not invoked by the compiler (despite the syntax leading us to believe the contrary). 

Being aware of this behavior is important and special care need to be taken when defining public APIs (specially in libraries due to the potential reach of those) to avoid ending up with APIs that are error prone due to uninitialized struct members.

That said, this behavior is not new; problems like the ones discussed in post [#5](https://programing-fun.blogspot.com/2023/12/structs-in-c-are-fun-part-59-other.html), [#6](https://programing-fun.blogspot.com/2024/01/structs-in-c-are-fun-part-69-struct.html) and [#7]() exists since the first versions of the language; however the introduction, in C# 10, of ability to define `parameterless` struct constructors adds more complexity and makes it more likely that developers will be exposed to such types[^1].

Bla bla bla

As always, all feedback is welcome.

Have fun!

[^1] : Prior to C# 10, the only way a developer would be exposed to such types would be to consume an assembly built with some language that has support for declaring parameterless constructors in structs.



