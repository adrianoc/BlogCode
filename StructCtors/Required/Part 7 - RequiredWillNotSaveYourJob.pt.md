[Lire cet post en français.]()

[Read this post in english]()

1. [Structs em C# - diversão garantida](https://programing-fun.blogspot.com/2023/06/structs-em-c-diversao-garantida-parte-19.html)
1. [Rápida introdução à Value Types vs Reference Types](https://programing-fun.blogspot.com/2023/07/structs-em-c-diversao-garantida-parte.html)
1. [Inicialização de campos em estruturas](https://programing-fun.blogspot.com/2023/08/structs-em-c-diversao-garantida-parte.html)
1. [Comportamento de construtores em estruturas](https://programing-fun.blogspot.com/2023/11/structs-em-c-diversao-garantida-parte.html).
1. [Outros cenários em que o comportamento de construtores em estruturas podem te surpreender](https://programing-fun.blogspot.com/2023/12/structs-em-c-diversao-garantida-parte.html).
1. [Argumentos default em construtores de estruturas (você ainda não esta confuso ?).](https://programing-fun.blogspot.com/2024/01/structs-em-c-diversao-garantida-parte.html)
1. Modificador `required` do C# 11 não vai salvar seu ~~c*~~ trabalho.  (este post)
1. Estruturas usadas como valor default de argumentos.
1. Conclusão.

No decorrer desta série de posts, ao explorar o comportamento dos construtores em structs em C#, apresentamos alguns cenários em que tais construtores não são invocados pelo compilador (apesar da sintaxe nos levar a acreditar no contrário).

A história deste post é um pouco constrangedora; durante o processo de definição dos tópicos a serem abordados, me deparei com um recurso do C# 11 chamado [`required member`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required) o qual eu ingenuamente acreditei poder usar para sinalizar tais cenários, então planejei adicionar uma postagem mostrando como atingir tal object usando esta funcionalidade; contudo, durante a investigação/elaboração da postagem, percebi que esse não era um dos objetivos desse recurso e que haviam vários casos em que nenhum aviso seria emitido, mesmo que nenhum construtor fosse invocado[^1].

A ideia principal seria marcar todos os membros (campos/propriedades) a ser inicializados nos construtores como `required` e adicionar o atributo [`SetsRequiredMembers`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.requiredmemberattribute?view=net-8.0) a esses construtores de tal forma que, nos casos em que os mesmos não fossem executados, devido à não inicialização dos campos, o compilador emitiria um aviso/erro.

Esta técnica funciona bem se desejarmos capturar um dos casos mais problemáticos: `ValueTypes` sem nenhum _construtor sem parâmetros_ e pelo menos um construtor no qual todos os seus parâmetros são opcionais. Para exemplificar isso, vamos pegar nosso último exemplo da postagem anterior e modificá-lo conforme descrito na seção _ideia principal_ acima:

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

Uma vez aplicada estas mudanças, em vez de, silenciosamente instanciar `S2` com o valor zero (em oposição ao esperado 42 se um construtor fosse invocado), obtemos o seguinte erro:

>error CS9035: Required member 'S2.v' must be set in the object initializer or attribute constructor.

Não é perfeito, visto que a mensagem provavelmente será muito confusa se esperarmos (incorretamente) que o construtor `S2` seja invocado, mas há outros problemas que tornam esta abordagem limitada:

   - Falha ao detectar construtores que não estão sendo invocados (pelo menos) em _default expressions_ e _instanciações de array_.
   - Mesmo em cenários em que a técnica funcionaria é impossível garantir que um ctor será invocado (por exemplo, se mudarmos o código na linha #1 para `new S2() { v = 5 }`, o construtor ainda não será invocado, mas nenhum aviso/erro será emitido)

Com isso em mente, me parece que escrever um Roslyn Analyzer personalizado seria a melhor opção para verificar, em tempo de compilação, se um construtor de uma **struct** está sendo invocado, contudo não tenho certeza da viabilidade de realizar uma análise de fluxo que capture todos os possíveis cenários sem produzir muitos falsos positivos/negativos.

Como sempre, todo feedback é bem-vindo.

Divirta-se!

[^1]: Após me dar conta disto mudei o título do post.