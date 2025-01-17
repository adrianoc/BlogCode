[Leia este post em Português](https://programing-fun.blogspot.com/2023/05/uma-funcionalidade-do-cecilifier-pouco.html)

[Read this post in english](https://programing-fun.blogspot.com/2023/05/uma-funcionalidade-do-cecilifier-pouco.html)

1. [Les structures en C# sont amusantes](https://programing-fun.blogspot.com/2023/06/les-structures-en-c-sont-amusantes.html)
1. [Brève introduction aux Value Types vs Reference Types](https://programing-fun.blogspot.com/2023/07/les-structures-en-c-sont-amusantes.html).
1. [Initialisation des champs dans les structures](https://programing-fun.blogspot.com/2023/08/structs-in-c-are-fun-part-39-field.html).
2. [Comportement des constructeurs dans structures](https://programing-fun.blogspot.com/2023/11/les-structures-en-c-sont-amusantes.html).
1. [Des autres scénarios dans lesquels le comportement des constructeurs de structure peut vous surprendre](https://programing-fun.blogspot.com/2023/12/les-structures-en-c-sont-amusantes.html).
1. Struct avec des valeurs d'argument par défaut dans les constructeurs, ou, n'êtes-vous pas encore confus ? _(cet post)_
1. Le modificateur [required](https://github.com/dotnet/csharplang/issues/3630) de C # 11 ne sauvegardera pas votre ~~c*l~~ emploi.
1. Structure utilisée comme valeurs d'argument défaut.
1. Bonus: L'evolution des structures en C#.

Dans les [derniere](https://programing-fun.blogspot.com/2023/11/les-structures-en-c-sont-amusantes.html) [posts](https://programing-fun.blogspot.com/2023/12/les-structures-en-c-sont-amusantes.html), nous avons vu que le compilateur C# peut net pas exécuter le constructeur des structs dans certains scénarios. Malheureusement, l'expérience avec les constructeurs dans les structures, du moins à mon avis, peut devenir encore plus déroutante.

Pour illustrer mon point, supposons que vous ayez le code suivant :

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

Quel résultat attendez-vous s’il est compilé/exécuté[^1] ?

1. Le compilateur émettra une erreur en affirmant que `new S2()` est un appel ambigu.
1. Il compile et génère 42, 13.
1. Il compile et génère 84, 13.

La bonne réponse, qui peut surprendre certains développeurs, est qu'il se compile avec succès et affiche `84` et `13` (c'est-à-dire la troisième option).

Cela se produit parce qu'à la ligne 1, le compilateur C# considère le constructeur sans paramètre comme une _meilleure correspondance_ (better match) que celui avec la valeur de paramètre par défaut excluant les options 1 et 2[^1]. Le bon côté est que même si cela n'est pas totalement évident, au moins ce comportement est cohérent entre les classes/structures.

Pourtant, avec les structures, cela peut devenir encore plus complexe/déroutant ; imaginez que vous avez exactement le même code que ci-dessus, la seule différence étant la suppression du constructeur sans paramètre :

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

maintenant, c'est sûr qu'il imprimera 42, 13, n'est-ce pas ?

Non! Dans ce scénario, contrairement aux classes, un constructeur avec des valeurs par défaut pour tous ses paramètres n'est pas invoqué, même sur une `new expression` *explicite*[^2], ce qui signifie que le code ci-dessus imprimera `0` et `13`[^4].

Dans le prochain article, nous examinerons rapidement les [membres requis](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-11.0/required-members) C# 11[^3] comme moyen d'aider à identifier les scénarios dans lesquels aucun constructeur n'est invoqué.

Comme toujours, tous les commentaires sont les bienvenus.

Amusez-vous!

[^1]: Pour être honnête, le comportement ici est le même pour les classes, donc au moins il est cohérent.

[^2]: la spécification des constructeurs sans paramètre dans les structures indique explicitement que ce comportement est le même que dans les versions précédentes de C#.

[^3]: Je suis conscient que ce n'est pas l'utilisation prévue de cette fonctionnalité, mais elle peut être utilisée pour signaler quelques instanciations de structure qui n'invoquent pas de constructeur.

[^4]: Cela se produit parce que la ligne n°1 est équivalente à `Print(default(S2))` ; pour plus de détails, voir [cet article](https://programing-fun.blogspot.com/2023/11/structs-in-c-are-fun-part-49.html).