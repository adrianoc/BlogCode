<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/brazil_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;"><a href="https://programing-fun.blogspot.com/2025/01/structs.8.pt.html">Leia este post em Português.</a></p>

<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/france_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;" /><a href="https://programing-fun.blogspot.com/2025/01/structs.8.fr.html">Lire cet post en français.</a></p>

<script>
populateToc("https://raw.githubusercontent.com/adrianoc/BlogCode/refs/heads/main/StructCtors/toc.en.js", 'struct-series-toc');
</script>
<p id="struct-series-toc">

In a previous post we looked at having default parameter values **in struct constructors**; this time we are going to have a quick look on what happens when we have struct parameters with default values. So, without further ado lets start with the following struct[^1]: 

```csharp
struct S
{ 
    public int v = 42;
    public S(string s) { v = 13; }
}
```

and the following usage:

```csharp
M();

void M(S s = new S()) => Console.WriteLine(s.v);
```

What you expect the output to be? 

If you have been following this post series you probably know that this is a trick question and that neither *42* nor *13* are correct answers since that would require that either the *field initializer* and/or the constructor to be executed but the expression `new S()` used as the `default parameter value` does not implies a constructor invocation (after all there's no such parameterless ctor and if you feel tempted to add one the compiler will happily emit an error since in the presence of such ctor the expression in question does not represent a compile time constant anymore whence it cannot be used as a default parameter value).

When executed that code snippet will output `0` because the compiler simply initializes the full memory used to store the struct instance with zeros (`0`). By the other hand if you invoke method `M()` as follows[^2]

```csharp
M(new S("Foo"));
```

then the compiler will emit code to execute the constructor and the field initializer and `13` will be printed out, but this has nothing to do with the `default parameter value` anymore.

And with that we have explored the last `not so intuitive` struct behavior we intended to cover; next post will be the conclusion of this series.

As always, all feedback is welcome.

Have fun!

[^1]: Note that support for field initializers in structs was introduced in C# 10.
[^2]: The content of the string is not important in this context. All that matters is that a constructor is invoked.