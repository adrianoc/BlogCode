[Lire cet post en français.](https://programing-fun.blogspot.com/2023/05/little-unknown-cecilifier-feature.html)

[Read this post in English.](https://programing-fun.blogspot.com/2023/05/uma-funcionalidade-do-cecilifier-pouco.html)

<script>
populateToc("https://raw.githubusercontent.com/adrianoc/BlogCode/refs/heads/main/StructCtors/toc.pt.js", 'struct-series-toc');
</script>

<p id="struct-series-toc">

A história deste post é um pouco embaraçosa.

Durante o processo de definição dos tópicos que eu cobriria, aprendi sobre uma funcionalidade do C# 11 chamada [`required members`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required) que, ingenuamente, pensei poderia ser usada para sinalizar esses cenários, então planejei adicionar um post mostrando como alcançar isso; no entanto, durante a investigação/redação[^1] do conteúdo percebi que esse não era um dos objetivos dessa funcionalidade e que havia vários casos extremos em que nenhum aviso seria emitido, mesmo que nenhum construtor fosse invocado[^2].

A ideia principal era marcar todos os membros (campos/propriedades) que seriam inicializados nos construtores como `required` e adicionar o atributo [`SetsRequiredMembers`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.requiredmemberattribute?view=net-8.0) a esses construtores, de forma que, nos casos em que nenhum construtor (decorado com `SetRequiredMembersAttribute`) fosse invocado, o compilador emitiria um aviso/erro devido à não inicialização desses membros.

Essa técnica funciona relativamente bem se quisermos capturar um cenário muito problemático: a instanciação de tipos de valor com um construtor em que todos os seus parâmetros possuim valores opcionais[^3]. Para tornar a discussão mais concreta, vamos pegar nosso último exemplo do post anterior e modificá-lo conforme descrito acima:

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

Com essa mudança em vez de obter silenciosamente uma instância de `S2` inicializada com zero (em oposição ao esperado 42 se o construtor fosse invocado), o compiladore emite o seguinte erro:

>error CS9035: O membro obrigatório 'S2.v' deve ser definido no inicializador de objeto ou no construtor de atributo.

Não é perfeito, já que a mensagem provavelmente será muito confusa se alguém estiver (incorretamente, mas compreensivelmente) esperando que o construtor de `S2` seja invocado, mas há outras limitações que tornam essa abordagem ainda menos viável:

  - Incapacidade de detectar construtores não invocados (pelo menos) em expressões _default_ e instanciações de arrays.
  - Mesmo em cenários em que isso funcionaria, é impossível garantir que um construtor será invocado (por exemplo, se mudarmos o código na linha #1 para `new S2() { v = 5 }`, nenhum construtor será invocado, mas nenhum aviso/erro será emitido)

Uma alternativa mais eficaz (se você implantar sua aplicação como gerenciada em vez de compilá-la AOT) para detectar tais cenários é garantir explicitamente que instâncias de struct foram inicializadas[^4] (seja por um construtor ou por outros meios) antes de acessar seus membros; como o código está gerado em tempo de execução pelo JIT, é possível implementar isso de forma que os usuários possam controlar se tal verificação deve ser aplicada ou com muito pouco (ou nenhum) impacto em desempenho quando a verificação está desativada, como demonstrado abaixo.

```CSharp
using System.Runtime.CompilerServices;

class Driver
{
    static void Main()
    {

        for(int i = 0; i < 100_000; i++)
        {
            var foo = new Foo(); // Nenhum construtor invocado... nenhum aviso :(
            Thread.Sleep(100);
            foo.Use();
        }
    }

}

// A estrutura pode ser declarada em um assembly diferente também. 
struct Foo
{
    // é importante que o campo seja marcado como `readonly`
    private static readonly bool _shouldValidate = Environment.GetEnvironmentVariable("VALIDATE_FOO") == "true";

    private int _i;
    private bool _isInitialized;

    public Foo(int v = 1) { _i = v; _isInitialized = true; }

    public void Use()
    {
        Verify();
        System.Console.WriteLine(_i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // É importante pedir que o JIT inline este método para que a otimização seja aplicada.
    private readonly void Verify()
    {
        if (_shouldValidate)
        {
            if (!_isInitialized)
            {
                throw new Exception("O construtor de Foo não foi invocado; isso pode ser devido a uma declaração de array ou ...");
            }
        }
    }
}
```

O código usa o campo `_shouldValidate` para controlar se a verificação de inicialização[^4] deve ser aplicada ou não (neste caso, mais especificamente, se o construtor foi executado). Observe que o mesmo é declarado como `static readonly`; isso é muito importante pois assim o JIT sabe que, uma vez inicializada uma dada instância de struct, o valor do campo **nunca mudará**, podendo tratar `_shouldValidate` como uma constante não gerando código para verificá-lo no **if** na linha #XX; além disso, caso esta constante seja avaliada como **false**, o JIT pode remover o  `if` (incluindo seu corpo) completamente (daí o custo quase zero mencionado anteriormente).

Você pode ver essa *mágica* do JIT em ação abrindo um terminal, criando uma aplicação de console com o código acima e executando:

```bash
DOTNET_JitDisasm=Use dotnet run -c Release
```

o qual:

1. em sistemas operacionais do tipo `unix`, define a variável de ambiente `DOTNET_JitDisasm` como `Use`, o que instrui o JIT a fazer o dump do código assembly gerado para o método de mesmo nome.
1. compila a aplicação em **modo release** (`-c Release`), o que é um requisito para que a otimização seja aplicada.
1. executa a aplicação.

Ao executar `DOTNET_JitDisasm=Use dotnet run -c Release`, você deve ver zeros (0) e algum código assembly sendo impresso no terminal várias vezes; após algumas iterações, você deve ser capaz de identificar código assembly semelhante ao abaixo (certifique-se de verificar o que mesmo contém `Tier1` em vez de `Tier0`):

```nasm
; Assembly listing for method Foo:Use():this (Tier1)
; Emitting BLENDED_CODE for X64 with AVX - Unix
; Tier1 code
; optimized code
; rsp based frame
; fully interruptible
; No PGO data
; 1 inlinees with PGO data; 0 single block inlinees; 0 inlinees without PGO data

G_M000_IG01:                ;; offset=0x0000
 
G_M000_IG02:                ;; offset=0x0000
       mov      edi, dword ptr [rdi]
 
G_M000_IG03:                ;; offset=0x0002
       tail.jmp [System.Console:WriteLine(int)]
 
; Total bytes of code 8
```

que basicamente chama `System.Console.WriteLine(_i)` e retorna, sem nenhum traço da invocação do método `Verify()`.

Você também pode brincar com este exemplo executando-o como:

```bash
DOTNET_JitDisasm=Use VALIDATE_FOO=true dotnet run -c Release
```

nesse caso, ele lançará uma exceção (provando que o uso de instâncias de struct não inicializadas é detectado)

ou

```bash
DOTNET_JitDisasm=Use dotnet run -c Debug
```

não importando nesse caso quanto tempo a aplicação seja executada, o código assembly gerado para `Use()` sempre chamará `Verify()` (ou seja, a otimização não foi aplicada porque a aplicação foi compilada em **modo debug**)

Com essa abordagem, pode-se ter certeza de que nenhum código está usando instâncias `não inicializadas` simplesmente executando o código com a variável de ambiente definida como `true` e observando exceções.

Como sempre, todos os feedbacks são bem-vindos.

Divirta-se!

[^1]: [aqui](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwGIATGAMwEs8YACAVQGUBRAfQCVmBFegSU4AiAWABQAAQAMtMQEYAdAMoBDAOZ4IAZwyUwGuQGEIZAIJ4lAGwCeGyhoDcosQCYZo0QDclUWrACOtAF5aMnIlHHMMAAp2GF8ASjtaAHok2lcRT28/J0DaGgB3WhjfSITkpIBvWgA1Cxw6IIBWWgBfWkSU2icPLx9YgGZcgqLYyJkynqzYlCGYQuKxtC6JkVEU2QBOSL85WvN6hMcZLezduphD8WPtgbP9i4crk+m7g8e3ES1cMAwR31EKqIiJRyAwWBxuHxBLQYbRRLDpIM/DhKLASLRqL89vVHkQYOYNHRYfDYWJBpiaudcTA8CQQSSYQzaABtRgwDAaYootEAWRgAFtgDAoBoALpMsl/SIUyhLCkAKzigQAfJT7rl5YliSIEUyUqz2ZzYtyYCQ+YLhWK9UlJQslSq1fVcignI9dTrYQaOVzUabzUKReKPTDbaMKQhck4JPaAqrsQ1aAhHi0Ps5aAIKACJYMM+RpXhfpQlUzAcGEdJrgAiXO0H7QWsWcymyuXBEpkQtIA===) você pode encontrar alguns códigos de teste que usei enquanto explorava este tópico.

[^2]: Depois de perceber isso, mudei o título do post :).

[^3]: Este caso específico é problemático devido à expectativa de que o comportamento corresponderia ao comportamento para **classes**.

[^4]: Nota de esclarecimento: Do ponto de vista da runtime, structs são garantidas de serem inicializados (_zerando toda a struct_) antes de serem usadas. _Inicialização_ no contexto desta série significa que todos os campos/propriedades da struct foram atribuídos valores significativos, deixando a instância em um estado consistente.