[Leia este post em Português]()

[Read this post in english]()

1. [Les structures en C# sont amusantes](https://programing-fun.blogspot.com/2023/06/les-structures-en-c-sont-amusantes.html)
1. [Brève introduction aux Value Types vs Reference Types](https://programing-fun.blogspot.com/2023/07/les-structures-en-c-sont-amusantes.html).
1. [Initialisation des champs dans les structures](https://programing-fun.blogspot.com/2023/08/structs-in-c-are-fun-part-39-field.html).
2. [Comportement des constructeurs dans structures](https://programing-fun.blogspot.com/2023/11/les-structures-en-c-sont-amusantes.html).
1. [Des autres scénarios dans lesquels le comportement des constructeurs de structure peut vous surprendre](https://programing-fun.blogspot.com/2023/12/les-structures-en-c-sont-amusantes.html).
1. [Struct avec des valeurs d'argument par défaut dans les constructeurs, ou, n'êtes-vous pas encore confus ?](https://programing-fun.blogspot.com/2024/01/les-structures-en-c-sont-amusantes.html)
1. Le modificateur [required](https://github.com/dotnet/csharplang/issues/3630) de C # 11 ne sauvegardera pas votre ~~c*l~~ emploi. _(cet post)_
1. Structure utilisée comme valeurs d'argument défaut.
1. Conclusion.

Tout au long de cette série d'articles, en explorant le comportement des constructeurs dans les structures en C#, nous présentons quelques scénarios dans lesquels ces constructeurs ne sont pas invoqués par le compilateur (malgré la syntaxe nous laissant croire le contraire).

L’histoire du poste actuel est un peu embarrassante ; au cours du processus de définition des sujets que je couvrirais, j'ai découvert une fonctionnalité C# 11 appelée [`required Members`](https://learn.microsoft.com/en-us/dotnet/csharp/langage-reference/keywords/required) qui, je pensais naïvement, pourraient être utilisés pour signaler ces scénarios, j'ai donc prévu d'ajouter un article montrant comment y parvenir ; Qui que ce soit lors de l'enquête/rédaction du message, j'ai réalisé que ce n'était pas l'un des objectifs de cette fonctionnalité et qu'il y avait plusieurs cas particuliers dans lesquels aucun avertissement ne serait émis même si aucun constructeur ne serait invoqué[^1].

L'idée principale serait de marquer tous les membres (champs/propriétés) à initialiser dans les constructeurs comme « requis » et d'ajouter l'attribut [`SetsRequiredMembers`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.requiredmemberattribute?view=net-8.0) à ces constructeurs de telle sorte que, dans les cas où ils ne seraient pas exécutés, en raison de la non-initialisation des champs, le compilateur émettrait un avertissement/une erreur.

Cette technique fonctionne bien si nous voulons capturer l'un des cas les plus problématiques : `ValueTypes` sans _constructeur sans paramètre_ et au moins un constructeur dans lequel tous ses paramètres sont facultatifs. Pour illustrer cela, prenons notre dernier exemple du post précédent et modifions-le comme décrit dans la section _idée principale_ ci-dessus :

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

Une fois que nous appliquons ces modifications, au lieu d'instancier silencieusement « S2 » avec la valeur zéro (par opposition aux 42 attendus si un constructeur était invoqué), nous obtenons l'erreur suivante :

>error CS9035: Required member 'S2.v' must be set in the object initializer or attribute constructor.

Ce n'est pas parfait, car le message sera probablement très déroutant si nous attendons (à tort) que le constructeur `S2` soit invoqué, mais il existe d'autres problèmes qui rendent cette approche limitée :

    - Impossible de détecter les constructeurs qui ne sont pas invoqués (au moins) dans les _default expressions_ et les _instanciations de arrays_.
    - Même dans les scénarios où la technique fonctionnerait, il est impossible de garantir qu'un cteur sera invoqué (par exemple, si nous changeons le code de la ligne #1 en `new S2() { v = 5 }`, le constructeur ne sera toujours pas invoqué, mais aucun avertissement/erreur sera émis non plus)

Dans cet esprit, il me semble qu'écrire un Roslyn Analyzer personnalisé serait la meilleure option pour vérifier, au moment de la compilation, si un constructeur d'une **struct** est invoqué, mais je ne suis pas sûr de la faisabilité de effectuer une analyse de flux qui capture tous les scénarios possibles sans produire trop de faux positifs/négatifs.

Comme toujours, tous les commentaires sont les bienvenus.

Amusez-vous!

[^1]: Après avoir réalisé cela j'ai changé le titre du message :).