[Leia este post em Português]()

[Lire cet post en français.]()

<script>
populateToc("https://gist.githubusercontent.com/adrianoc/3e3a5159d438ea953c46f5a28a417a37/raw/179c99b21f9036026d8073a6e03e83c9b2aed1d4/struct_posts_toc.en.json", 'struct-series-toc');
</script>

<div id="struct-series-toc">

In the previous posts of this series, while exploring the behavior of constructors in structs in C# we've learned that in some scenarios these constructors are not executed (despite the syntax leading us to believe the contrary). 

Being aware of this behavior is important and special care should be taken when defining public APIs (specially in libraries due to the potential reach of those) to avoid ending up with APIs that are unintuitive and/or error prone due to developers easily getting into the trap of using uninitialized struct members.

That said, this behavior is not new; problems like the ones discussed in post [#5](https://programing-fun.blogspot.com/2023/12/structs-in-c-are-fun-part-59-other.html), [#6](https://programing-fun.blogspot.com/2024/01/structs-in-c-are-fun-part-69-struct.html) and [#7]() exists since the first versions of the language; however the introduction, in C# 10, of the ability to define `parameterless` constructors in structs adds more complexity and makes it more likely that developers will be exposed to  such types[^1].

So, to finish up this series, if you are consuming struct types be careful to not end up using uninitialized[^2] instances. By the other hand, take special care when defining new `struct types`, specially if you add a `parameterless ctor` or if it defines a _constructor with all of its parameters being optional_.

As always, all feedback is welcome.

Have fun!

[^1]: Prior to C# 10, the only way a developer would be exposed to such types would be by consuming assemblies built with a language supporting that feature, such as IL.

[^2]: Note of clarification: From the perspective of the runtime, structs are guaranteed to be initialized (by _zeroing out the whole struct_) before being used. _Initialization_ in the context of this series of posts means that all struct fields/properties have been assigned meaningful values leaving the instance in a consistent state. 



-- 
Nos posts anteriores desta série, enquanto explorávamos o comportamento de instânciação de structs em C# encontramos alguns cenários nos quais o processo de instanciação de tais tipos não levava à execução de seus construtores (apesar da sintaxe nos levar a acreditar o contrário)

Dans les précédents articles de cette série, tout en explorant le comportement d’instanciation des structures en C#, nous avons appris plusieurs scénarios dans lesquels les constructeurs ne sont pas invoqués lors de l’instanciation de ces types (bien que la syntaxe nous laisse croire le contraire).