<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/brazil_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;"><a href="https://programing-fun.blogspot.com/2025/03/structs.9.pt.html">Leia este post em Português.</a></p>

<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/france_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;" /><a href="https://programing-fun.blogspot.com/2025/03/structs.9.fr.html">Lire cet post en français.</a></p>

<script>
populateToc("https://raw.githubusercontent.com/adrianoc/BlogCode/refs/heads/main/StructCtors/toc.en.js", 'struct-series-toc-9-en');
</script>
<p id="struct-series-toc-9-en">

In the previous posts of this series, while exploring the behavior of constructors in structs in C# we've learned that in some scenarios these constructors are not executed (despite the syntax leading us to believe the contrary).

Being aware of this behavior is important and special care should be taken when defining public APIs (specially in libraries due to the potential reach of those) to avoid ending up with APIs that are unintuitive and/or error prone due to developers easily falling into the trap of using uninitialized struct[^1] members.

That said, this behavior is not new; problems like the ones discussed in post [#5](https://programing-fun.blogspot.com/2023/12/structs-in-c-are-fun-part-59-other.html), [#6](https://programing-fun.blogspot.com/2024/01/structs-in-c-are-fun-part-69-struct.html) and [#7](https://programing-fun.blogspot.com/2025/01/structs.7.en.html) existed since the first versions of the language; however the introduction, in C# 10, of the ability to define `parameterless` constructors in structs adds more complexity and makes it more likely that developers will be exposed to such types[^2].

So, if you are consuming struct types be careful to not end up using uninitialized[^1] instances. By the other hand, take special care when defining new `struct types`, specially those with `parameterless constructors` or a `constructor with all of its parameters being optional`.

With this post we've reached the last one in this series; I hope you've enjoyed and possibly learned something new in the journey and now are better prepared to both use as well expose your own structs.

As always, all feedback is welcome.

Have fun!

[^1]: Note of clarification: From the perspective of the runtime, structs are guaranteed to be initialized (by _zeroing out the whole struct_) before being used. _Initialization_ in the context of this series means that all struct fields/properties have been assigned meaningful values leaving the instance in a consistent state.

[^2]: Prior to C# 10, the only way a developer would be exposed to such types would be by consuming assemblies built with a language supporting that feature, such as IL; personally I am not aware of a single instance of such types.