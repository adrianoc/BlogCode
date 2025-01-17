[Lire cet post en français.](https://programing-fun.blogspot.com/2023/05/little-unknown-cecilifier-feature.html)

[Read this post in english](https://programing-fun.blogspot.com/2023/05/uma-funcionalidade-do-cecilifier-pouco.html)

1. [Structs em C# - diversão garantida](https://programing-fun.blogspot.com/2023/06/structs-em-c-diversao-garantida-parte-19.html)
1. [Rápida introdução à Value Types vs Reference Types](https://programing-fun.blogspot.com/2023/07/structs-em-c-diversao-garantida-parte.html)
1. [Inicialização de campos em estruturas](https://programing-fun.blogspot.com/2023/08/structs-em-c-diversao-garantida-parte.html)
1. [Comportamento de construtores em estruturas](https://programing-fun.blogspot.com/2023/11/structs-em-c-diversao-garantida-parte.html).
1. [Outros cenários em que o comportamento de construtores em estruturas podem te surpreender](https://programing-fun.blogspot.com/2023/12/structs-em-c-diversao-garantida-parte.html).
1. Argumentos default em construtores de estruturas (você ainda não esta confuso ?). (este post)
1. Modificador `required` do C# 11 não vai salvar seu ~~c*~~ trabalho.
1. Estruturas usadas como valor default de argumentos.
1. Bonus: Evolução das estruturas em C#.

Nos [posts](https://programing-fun.blogspot.com/2023/11/structs-em-c-diversao-garantida-parte.html) [anteriores](https://programing-fun.blogspot.com/2023/12/structs-em-c-diversao-garantida-parte.html) vimos que, em alguns cenários, o compilador C# pode não emitir invocaçcões de construtores de estruturas. Infelizmente a experiência com construtores em structs, pelo menos na minha opinião, pode ficar ainda mais confusa.

Para ilustrar o meu ponto, suponha que você tenha o seguinte código:

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

Qual resultado você espera observar ao compilar/executar o mesmo? [^1]

1. O compilador emitirá um erro informando que `new S2()` representa uma chamada ambígua.
1. Ele compila e gera 42, 13.
1. Ele compila e gera 84, 13.

A resposta correta, que pode surpreender alguns desenvolvedores, é que o programa compila com sucesso e imprime `84` e `13` (ou seja, a terceira opção).

Isso acontece porque na linha 1 o compilador C# vê o construtor sem parâmetros como uma _melhor correspondência (best match)_ que aquele com o valor do parâmetro default, descartando as opções 1 e 2[^1]. O lado bom é que mesmo não sendo totalmente óbvio, pelo menos este comportamento é consistente entre classes/estruturas.

No entanto, o comportamento de construtores em estruturas pode ficar ainda mais complexo/confuso; imagine que você tem exatamente o mesmo código acima, com a única diferença sendo a remoção do construtor sem parâmetros:

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

agora, com certeza o mesmo vai imprimir `42` e `13`, certo?

Não! Neste cenário, diferentemente das classes, um construtor com valores padrão para todos os seus parâmetros não é invocado, mesmo em expressões `new` *explícitas*[^2] o que significa que o código acima irá imprimir `0` e `13`[^4].

Na próxima postagem, daremos uma olhada rápida em uma funcionalide do C# 11 conhecida como [required members](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-11.0/required-members)[^3] como uma forma de ajudar a identificar cenários nos quais nenhum construtor é invocado.

Como sempre, todo feedback é bem-vindo.

Divirta-se!

[^1]: Para ser justo, o comportamento aqui é consistente com o de classes.

[^2]: A especificação de construtores sem parâmetros em estruturas [afirma explicitamente que esse comportamento é o mesmo das versões anteriores do C#](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/parameterless-struct-constructors#new).

[^3]: Estou ciente de que este não é o uso pretendido deste recurso, contudo o mesmo pode ser usado para sinalizar algumas instanciações de estruturas que não invocam um construtor.

[^4]: Isso acontece porque a linha #1 é equivalente a `Print(default(S2))`; para obter mais detalhes, consulte [esta postagem](https://programing-fun.blogspot.com/2023/11/structs-in-c-are-fun-part-49.html).