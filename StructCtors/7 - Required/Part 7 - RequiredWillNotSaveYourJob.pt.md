[Lire cet post en français.](https://programing-fun.blogspot.com/2023/05/little-unknown-cecilifier-feature.html)

[Read this post in English](https://programing-fun.blogspot.com/2023/05/uma-funcionalidade-do-cecilifier-pouco.html)

<script>
populateToc("https://gist.githubusercontent.com/adrianoc/d3e3071912c2491889ac99566bd21c49/raw/1113bd226117e32a48a145dc2883bf697b3979ca/struct_posts_toc.pt.json", 'struct-series-toc');
</script>

<p id="struct-series-toc">

A história deste post é um pouco embaraçosa; durante o processo de definição dos tópicos que eu cobriria, tomei conhecimento de uma funcionalidade do C# 11 chamada [`required members`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required) que eu ingenuamente pensei que poderia ser usada para sinalizar esses cenários, então planejei adicionar um post mostrando como fazê-lo. No entanto, durante a investigação/redação[^1] do conteúdo percebi que esse não era um dos objetivos dessa funcionalidade e que haviam vários casos em que nenhum aviso seria emitido, mesmo que nenhum construtor fosse invocado[^2].

A ideia principal seria marcar todos os membros (campos/propriedades) que seriam inicializados nos construtores como `required` e adicionar o atributo [`SetsRequiredMembers`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.requiredmemberattribute?view=net-8.0) a esses construtores de forma que, em caso de não execução do construtor e consequente não inicialização dos campos, o compilador emitisse um aviso/erro.

Essa técnica funciona relativamente bem se o objetivo for capturar um dos casos mais problemáticos: tipos de valor com um construtor no qual todos os seus parâmetros são opcionais[^3]. Para tornar a discussão mais concreta, vamos retomar o último exemplo do post anterior e modificá-lo como descrito acima:

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

Com essa mudança, ao invés de obter silenciosamente uma instância de `S2` inicializada com zero (ao contrário do esperado, 42, caso o construtor fosse invocado), obtemos o seguinte erro:

>erro CS9035: O membro requerido 'S2.v' deve ser definido no inicializador de objeto ou no construtor de atributo.

Não é perfeito, considerando que a mensagem provavelmente será bastante confusa se alguém (incorretamente, mas de forma compreensível) estiver esperando que o construtor de `S2` seja invocado, mas há outras limitações que tornam essa abordagem ainda menos viável:

  - Falha ao detectar construtores que não são invocados (pelo menos) em _expressões padrão_ e _instanciações de arrays_.
  - Mesmo em cenários nos quais isso funcionaria, é impossível garantir que um construtor será invocado (por exemplo, se mudarmos o código na linha #1 para `new S2() { v = 5 }`, o construtor ainda não será invocado, mas nenhum aviso/erro será emitido).

Com isso em mente, acho que escrever uma Roslyn analyzer personalizado seria a melhor opção para verificar, em tempo de compilação, se um construtor de struct será invocado ou não, mas não tenho certeza de quão viável seria garantir que todos os cenários seriam capturados, ao mesmo tempo, mantendo um baixo número de ocorrências de falsos positivos/negativos.

Como sempre, todos os comentários são bem-vindos.

Divirta-se!

[^1]: [aqui](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwGIATGAMwEs8YACAVQGUBRAfQCVmBFegSU4AiAWABQAAQAMtMQEYAdAMoBDAOZ4IAZwyUwGuQGEIZAIJ4lAGwCeGyhoDcosQCYZo0QDclUWrACOtAF5aMnIlHHMMAAp2GF8ASjtaAHok2lcRT28/J0DaGgB3WhjfSITkpIBvWgA1Cxw6IIBWWgBfWkSU2icPLx9YgGZcgqLYyJkynqzYlCGYQuKxtC6JkVEU2QBOSL85WvN6hMcZLezduphD8WPtgbP9i4crk+m7g8e3ES1cMAwR31EKqIiJRyAwWBxuHxBLQYbRRLDpIM/DhKLASLRqL89vVHkQYOYNHRYfDYWJBpiaudcTA8CQQSSYQzaABtRgwDAaYootEAWRgAFtgDAoBoALpMsl/SIUyhLCkAKzigQAfJT7rl5YliSIEUyUqz2ZzYtyYCQ+YLhWK9UlJQslSq1fVcignI9dTrYQaOVzUabzUKReKPTDbaMKQhck4JPaAqrsQ1aAhHi0Ps5aAIKACJYMM+RpXhfpQlUzAcGEdJrgAiXO0H7QWsWcymyuXBEpkQtIA===) você pode encontrar o código de teste que usei enquanto explorava este tópico.

[^2]: Após perceber isso, mudei o título do post :).

[^3]: Este caso particular é problemático devido à expectativa de que o comportamento seria o mesmo das classes.