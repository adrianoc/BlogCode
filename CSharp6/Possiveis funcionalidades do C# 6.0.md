Possiveis funcionalidades do C# 6.0
==================================

Sei que alguns desenvolvedores verão este post como notícia velha, mas eu me deparei com estas informaçoes hoje e as achei bem interessante e acredito que estas podem ser úteis a mais desenvolvedores (mesmo porque não encontrei muito sobre o assunto em português), assim sendo, decidi postar sobre o assunto.

Se você deseja ver os posts originais onde encontrei tais informações (em inglês) veja [estes][1] [links][2] (ambos são baseados em um evento da Microsoft que ocorreu em Londres chamado [NDC][3] que ocorreu em Dez/2013).

Os comentários representam a minha opnião (que pode não representar nada para você :)) e eu não sou, de forma alguma, um especialista em linguagens de programação (por exemplo não fiz esforço algum para verificar se as minhas sugestões não introduziriam ambiguidades na sintaxe da linguagem).

{/
	+git:GitContent -> HtmlEncode
	+sh:SyntaxHighlighter
	+nb:NiceBox
/}


"Primary constructors"
---------------------

{| git=CSharp6/5.0/PrimaryConstructors.cs sh.toolbar=false nb.lang=C# 5.0 |}

{| git=CSharp6/6.0/PrimaryConstructors.cs nb.lang=C# 6.0 sh.toolbar=false |}

- Não ficou claro para mim
	- será possível ter um corpo no construtor ?
	- paramêtros com valor default ?
	- assume a mesma visibilidade da classe ?
	- será possivel definir outros construtores ?

- Já que o objetivo é simplificar porque não introduzir um conceito tipo "campos automáticos" ou mesmo "propriedades automáticas" permitindo assim reescrever o exemplo como: 

{| git=CSharp6/6.0/PrimaryConstructors.cs@primary-ctor-proposal nb.lang=C# 6.0 sh.toolbar=false |}

Propriedades (automaticas) somente leitura 
------------------------------------------

{| git=CSharp6/5.0/AutomaticReadOnlyProperties.cs nb.lang=C# 5.0 sh.toolbar=false |}

{| git=CSharp6/6.0/AutomaticReadOnlyProperties.cs nb.lang=C# 6.0 sh.toolbar=false |}

> Eu realmente não vejo muito ganho com isto. Eu prefiro a versão abaixo (já suportada atualment é claro)
(ok, eu sei que as mesmas não semanticamente equivalentes)

{| git=CSharp6/5.0/AutomaticReadOnlyProperties.cs@today nb.lang=C# 5.0 sh.toobar=false |}

- O que pode ser usado como valor da propriedade? Qualquer expressão? 

{| git=CSharp6/6.0/AutomaticReadOnlyProperties.cs@what-is-supported nb.lang=C# 6.0 sh.toolbar=false |}

----
"Using" para membros estáticos

{| git=CSharp6/5.0/StaticMemberUsings.cs nb.lang=C# 5.0 sh.toolbar=false |}

{| git=CSharp6/6.0/StaticMemberUsings.cs nb.lang=C# 6.0 sh.toolbar=false |}

Sério? Java já suporta este recurso a algum tempo... e eu nunca gostei muito do mesmo... :(


----
Property Expressions (cara, não sei nem como traduzir isso)

{| git=CSharp6/5.0/PropertyExpressions.cs nb.lang=C# 5.0 sh.toolbar=false |}

{| git=CSharp6/6.0/PropertyExpressions.cs nb.lang=C# 6.0 sh.toolbar=false |}

- Ok, na minha opnião vivemos muito bem sem este recurso.

- Péssimo nome (vai causar confusão)

----
"Method expressions"

C# 5.0

class Exemplo1
{
	public Point Move(int dx, int dy) 
	{ 
		return new Point(X + dx, Y + dy); 
	}
}

C# 6.0

class Exemplo1
{
	public Point Move(int dx, int dy) => new Point(X + dx, Y + dy);
}

Hum... 


----
Enumerables como parâmetros do tipo "params"

{| git=CSharp6/5.0/EnumerableParams.cs nb.lang=C# 5.0 sh.toolbar=false |}

{| git=CSharp6/6.0/EnumerableParams.cs nb.lang=C# 6.0 sh.toolbar=false |}

Ok, dessa eu gostei pois não me obriga criar arrays só para poder chamar métodos como no exemplo acima (lembre-se que arrays em C# implementam IEnumerable<T> então, passar arrays, para métodos definidos como o exemplo acima, não exige conversões!

----
Checagem de "null" mais inteligente (ou "monadic null checking", o que quer que isso queira dizer ;)

{| git=CSharp6/5.0/MonadicNullChecking.cs nb.lang=C# 5.0 sh.toolbar=false |}

{| git=CSharp6/6.0/MonadicNullChecking.cs nb.lang=C# 6.0 sh.toolbar=false |}

Vamos acabar com o Natal! (afinal de contas, este tipo de sintaxe ajudaria em muito a por um fim em código tipo arvore de natal).

Ponto para a equipe do C#

----
Inferência de tipos a partir de parêmtros de construtores

{| git=CSharp6/5.0/GenericTypeInferenceInCtors.cs nb.lang=C# 5.0 sh.toolbar=false |}

{| git=CSharp6/6.0/GenericTypeInferenceInCtors.cs nb.lang=C# 6.0 sh.toolbar=false |}

Preciso dizer algo? Mais um ponto para os designer da linguagem

----
Declaração "inline" para parâmetros "out"

{| git=CSharp6/5.0/InlineOutParameters.cs nb.lang=C# 5.0 sh.toolbar=false |}

{| git=CSharp6/5.0/InlineOutParameters.cs nb.lang=C# 6.0 sh.toolbar=false |}

Hum.. este recurso me deixou em dúvida.


Mesmo que não seja relacionado exclusivamente ao C#, de acordo com Anders Hejlsberg ([http://channel9.msdn.com/Events/Build/2013/9-006][neste vídeo] +- 00:34), é possível que o projeto conhecido como [http://msdn.microsoft.com/en-us/vstudio/roslyn.aspx][Roslyn] também será liberado na próxima versão do Visual Studio! Se você ainda não deu uma olhada no mesmo, eu recomendo pegar alguns minutos (possivelmente algumas horas :)) para fazê-lo! 

------

Agora que já discuti as possíveis funcionalidades da próxima versão do C# (pelo menos, segundo os posts que eu encontrei), gostaria de expor minha lista de desejos pessoal (ou seja, algumas funcionalidades que eu realmente gostaria de ver na próxima versão do C#):


## Classes anônimas implementando interfaces

Se você já desenvolveu em Java provavelmente já fez uso deste recurso e possivelmente sabe o potencial para diminuir a quantidade de código escrito bem com simplificá-lo. Sinceramente, este é uma das poucas funcionalidades de Java que eu sinto falta no C#. Se você não tem idéia do que estou falando veja o exemplo abaixo:

{| git=CSharp6/AnonymousInterfaceImplementation.cs nb.lang=C# Algum dia sh.toolbar=false |}

## Extension properties

Da mesma forma que extension methods podem simplificar, e muito, algums cenários de programação, acredito que o mesmo conceito aplicado à proriedades seria bastante útil (pelo menos eu já me deparei com algumas cituações onde o mesmo seria realmente muito útil).

{| git=CSharp6/ExtensionProperties.cs nb.lang=Proposta sh.toolbar=false |}



## Interpolação de strings

[http://en.wikipedia.org/wiki/String_interpolation][Interpolação de strings] permite a substituição de uma (ou mais) parte(s) de uma string pelo valor de uma variável de uma forma simples e intuitiva.

Abaixo apresento um exemplo hipotético caso o conceito fosse suportado em C#:

{| git=CSharp6/StringInterpolation.cs nb.lang=Proposta sh.toolbar=false |}

O mesmo exemplo, hoje, tem que ser escrito como:

{| git=CSharp6/StringInterpolation.cs@si-today nb.lang=C# 5.0 sh.toolbar=false |}

Eu, apesar de algumas reservas devido ao potencial de confusão em casos onde strings referenciam variáveis declaradas em um ponto distante, gostaria de ver esta funcionalidade na próxima versão do C#!


Boa programação!

[1]: http://damieng.com/blog/2013/12/09/probable-c-6-0-features-illustrated
[2]: http://wesnerm.blogs.com/net_undocumented/2013/12/mads-on-c-60.html
[3]: http://ndclondon.oktaset.com/t-11783