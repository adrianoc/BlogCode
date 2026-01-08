<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/united_kingdom_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;"><a href="https://programing-fun.blogspot.com/2025/03/structs.9.en.html">Read this post in English.</a></p>

<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/france_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;" /><a href="https://programing-fun.blogspot.com/2025/03/structs.9.fr.html">Lire cet article en français.</a></p>

<script>
populateToc("https://raw.githubusercontent.com/adrianoc/BlogCode/refs/heads/main/StructCtors/toc.pt.js", 'struct-series-toc-9-pt');
</script>
<p id="struct-series-toc-9-pt">

Nos posts anteriores desta série, enquanto explorávamos o comportamento de instânciação de structs em C# encontramos alguns cenários nos quais o processo de instanciação de tais tipos não levava à execução de seus construtores (apesar da sintaxe nos levar a acreditar o contrário)

Ciencia desse comportamento bem como tomar especial cuidado ao definir APIs públicas (principalmente em bibliotecas, devido ao potencial do grande alcance das mesmas) é essencial para evitar criar APIs que sejam contraintuitivas e/ou propensas a erros uma vez que desenvolvedores podem facilmente ser supreendidos ao usar membros de `structs` não inicializados introduzindo bugs em seus programas.

Dito isso, esse comportamento não é novo, e problemas como os discutidos nos posts [#5](https://programing-fun.blogspot.com/2023/12/structs-in-c-are-fun-part-59-other.html), [#6](https://programing-fun.blogspot.com/2024/03/structs-in-c-are-fun-part-69-struct.html) e [#7](https://programing-fun.blogspot.com/2025/01/structs.7.pt.html) existem desde as primeiras versões da linguagem. No entanto, a introdução da capacidade de definir construtores **sem parâmetros** em `structs`  no C# 10 adiciona uma camada extra de complexidade e torna mais provável que desenvolvedores sejam expostos a tais tipos[^1].  

Portanto, se você usar tipos `struct`, garanta que todas as instâncias das mesmas sejam corretamente inicializadas[^2]. Por outro lado preste especial atenção ao definir novos tipos `struct`, principalmente se estes tipos possuirem um **construtor sem parâmetros** ou um **construtor cujos parâmetros sejam todos opcionais**.

Por fim, espero que você tenha gostado desta série e que tenha aprendido pelo menos uma nova característica dos `structs`, permitindo que você os utilize e os exponha de forma mais eficiente e segura.

Com este post, chegamos ao último da série, mas isso não significa que cobrimos todas as peculiaridades dos `structs` (pretendo voltar a esse assunto no futuro).  

Como sempre, qualquer feedback é bem-vindo.  

Divirta-se!  

[^1]: Antes do C# 10, a única forma de um desenvolvedor ser exposto a esses tipos era consumindo assemblies compilados com uma linguagem que suportasse esse recurso, como IL; pessoalmente, nunca encontrei um exemplo desse tipo.  

[^2]: **Nota de esclarecimento**: Do ponto de vista do runtime, os `structs` são garantidos como inicializados (zerando toda a estrutura) antes de serem usados. O termo _inicialização_, no contexto desta série de posts, significa que **todos os campos/propriedades do `struct` receberam valores significativos**, deixando a instância em um estado consistente.