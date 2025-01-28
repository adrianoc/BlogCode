[Lisez ce post en Portugais](https://programing-fun.blogspot.com/2023/05/uma-funcionalidade-do-cecilifier-pouco.html)

[Lire cet article en français.](https://programing-fun.blogspot.com/2023/05/little-unknown-cecilifier-feature.html)

1. [Les structures en C# sont amusantes](https://programing-fun.blogspot.com/2023/06/structs-in-c-are-fun-part-19.html)
1. [Brève introduction aux types valeur vs types référence](https://programing-fun.blogspot.com/2023/07/structs-in-c-are-fun-part-29-brief.html)
1. [Initialisation des champs dans les structures](https://programing-fun.blogspot.com/2023/08/structs-in-c-are-fun-part-39-field.html)
1. [Constructeurs et comportement des structures](https://programing-fun.blogspot.com/2023/11/structs-in-c-are-fun-part-49.html)
1. [Autres scénarios où le comportement des constructeurs de structures peut vous surprendre](https://programing-fun.blogspot.com/2023/12/structs-in-c-are-fun-part-59-other.html).
1. [Structure avec des valeurs par défaut pour les arguments des constructeurs, alias, vous n'êtes pas encore confus ?](https://programing-fun.blogspot.com/2024/01/structs-in-c-are-fun-part-69-struct.html).
1. La fonctionnalité `required` de C# 11 ne sauvera pas votre ~~travail~~. (_cet article_)
1. Structures utilisées comme valeurs par défaut pour les arguments.
1. Conclusion.

L’histoire de cet article est un peu embarrassante ; au cours du processus de définition des sujets que je devais couvrir, j’ai appris l’existence d’une fonctionnalité de C# 11 appelée [`required members`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required) que j’ai naïvement cru pouvoir être utilisée pour signaler ces scénarios. J'avais donc prévu d’ajouter un post montrant comment y parvenir ; cependant, lors de l'investigation/redaction[^1] du contenu pour cet article, j’ai réalisé que ce n'était pas l’un des objectifs de cette fonctionnalité et qu’il existait de nombreux cas particuliers où aucun avertissement ne serait émis, même si aucun constructeur n’était invoqué[^2].

L'idée principale était de marquer tous les membres (champs/propriétés) qui seraient initialisés dans les constructeurs comme `required` et d'ajouter l'attribut [`SetsRequiredMembers`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.requiredmemberattribute?view=net-8.0) à ces constructeurs de manière à ce que, à cause de l'initialisation manquante des champs, le compilateur émette un avertissement/erreur dans les cas où aucun constructeur n'était invoqué.

Cette technique fonctionne relativement bien si l’on veut détecter un cas trés problématique : les types valeur avec un constructeur dans lequel tous ses paramètres sont optionnels[^3]. Pour rendre la discussion plus concrète, prenons notre dernier exemple du post précédent et modifions-le comme décrit ci-dessus :

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

Avec ce changement, au lieu d'obtenir silencieusement une instance de `S2` initialisée à zéro (contre l’attendu de 42 si le constructeur était invoqué), nous obtenons l'erreur suivante :

>error CS9035 : Le membre requis 'S2.v' doit être défini dans l'initialiseur d'objet ou le constructeur de l'attribut.

Pas parfait, étant donné que le message sera probablement très déroutant si l’on (incorrectement mais compréhensiblement) s'attend à ce que le constructeur de `S2` soit invoqué, mais il existe d'autres limitations rendant cette approche encore moins viable :

  - Impossible de détecter que les constructeurs ne sont pas invoqués (du moins) dans les _expressions par défaut_ et les _instanciations de tableaux_.
  - Même dans les scénarios où cela fonctionnerait, il est impossible de garantir qu'un constructeur sera invoqué (par exemple, si l’on change le code à la ligne #1 pour `new S2() { v = 5 }`, le constructeur ne sera toujours pas invoqué mais aucun avertissement/erreur ne sera émis).

Cela dit, je pense qu’écrire un analyseur Roslyn personnalisé serait la meilleure option pour vérifier, à la compilation, qu’un constructeur de structure est invoqué, mais je ne suis pas sûr de la faisabilité pour garantir qu’il pourrait couvrir tous les scénarios et, en même temps, ne pas générer trop de faux positifs/négatifs.

Comme toujours, tous les retours sont les bienvenus.

Amusez-vous bien !

[^1]: [ici](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwGIATGAMwEs8YACAVQGUBRAfQCVmBFegSU4AiAWABQAAQAMtMQEYAdAMoBDAOZ4IAZwyUwGuQGEIZAIJ4lAGwCeGyhoDcosQCYZo0QDclUWrACOtAF5aMnIlHHMMAAp2GF8ASjtaAHok2lcRT28/J0DaGgB3WhjfSITkpIBvWgA1Cxw6IIBWWgBfWkSU2icPLx9YgGZcgqLYyJkynqzYlCGYQuKxtC6JkVEU2QBOSL85WvN6hMcZLezduphD8WPtgbP9i4crk+m7g8e3ES1cMAwR31EKqIiJRyAwWBxuHxBLQYbRRLDpIM/DhKLASLRqL89vVHkQYOYNHRYfDYWJBpiaudcTA8CQQSSYQzaABtRgwDAaYootEAWRgAFtgDAoBoALpMsl/SIUyhLCkAKzigQAfJT7rl5YliSIEUyUqz2ZzYtyYCQ+YLhWK9UlJQslSq1fVcignI9dTrYQaOVzUabzUKReKPTDbaMKQhck4JPaAqrsQ1aAhHi0Ps5aAIKACJYMM+RpXhfpQlUzAcGEdJrgAiXO0H7QWsWcymyuXBEpkQtIA===) vous pouvez trouver un exemple de code de test que j'ai utilisé en explorant ce sujet.

[^2]: Après avoir réalisé cela, j'ai changé le titre de l'article :).
[^3]: Ce cas particulier est problématique en raison de l'attente que le comportement soit le même que pour les classes. 
