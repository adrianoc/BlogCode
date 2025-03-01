<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/united_kingdom_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;"><a href="https://programing-fun.blogspot.com/2025/01/structs.8.en.html">Read this post in English.</a></p>

<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/france_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;" /><a href="https://programing-fun.blogspot.com/2025/01/structs.8.fr.html">Lire cet article en français.</a></p>

<script>
populateToc("https://raw.githubusercontent.com/adrianoc/BlogCode/refs/heads/main/StructCtors/toc.pt.js", 'struct-series-toc');
</script>
<p id="struct-series-toc">

Em um post anterior examinamos o uso de parâmetros com valores *default* **em construtores de structs**; desta vez, vamos dar uma olhada rápida no que acontece quando temos parâmetros do tipo _struct_ com valores *default*. Então, sem mais delongas, vamos começar com a seguinte struct[^1]:

```csharp
struct S
{
    public int v = 42;
    public S(string s) { v = 13; }
}
```

e o seguinte uso:

```csharp
M();

void M(S s = new S()) => Console.WriteLine(s.v);
```

Qual saída você espera obter?

Se você tem acompanhado esta série de posts provavelmente notou que esta é uma pergunta capciosa e que nem *42* nem *13* são respostas corretas uma vez que para que tais valores pudessem ser impressos isso exigiria que o *inicializador de campo* e/ou o construtor fossem executados, mas a expressão `new S()` usada como `valor default em parâmetros` não implica uma invocação de construtor (afinal, não há um construtor sem parâmetros e, caso você esteja tentado a adicionar um, o compilador emitirá um erro, pois, na presença de tal construtor, a expressão em questão não representa mais uma constante de tempo de compilação, portanto, não pode ser usada como um valor **default**).

Na realidade, quando executado o trecho de código exibirá `0` pois o compilador simplesmente inicializa a memória usada para armazenar a instância da struct com zeros (`0`). Por outro lado, se você invocar o método `M()` como a seguir[^2]:

```csharp
M(new S("Foo"));
```

o compilador emitirá código para executar o construtor e o inicializador de campo e `13` será impresso, mas isso não tem mais nada a ver com o `valor de parâmetro padrão`.

E com isso exploramos o último comportamento `não tão intuitivo` de estruturas que pretendíamos abordar; o próximo post será a conclusão desta série.

Como sempre, todos os comentários são bem-vindos.

Divirta-se!

[^1]: Observe que o suporte para inicializadores de campo em structs foi introduzido no C# 10.
[^2]: O conteúdo da string não é importante neste contexto. Tudo o que importa é que um construtor seja invocado.