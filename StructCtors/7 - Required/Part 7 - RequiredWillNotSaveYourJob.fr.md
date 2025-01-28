[Leia este post em Português.](https://programing-fun.blogspot.com/2023/05/uma-funcionalidade-do-cecilifier-pouco.html)

[Read this post in English](https://programing-fun.blogspot.com/2023/05/little-unknown-cecilifier-feature.html)

<script>
populateToc("https://raw.githubusercontent.com/adrianoc/BlogCode/refs/heads/main/StructCtors/toc.fr.js", 'struct-series-toc');
</script>

<p id="struct-series-toc">

L'histoire de ce post est un peu embarrassante.

Pendant le processus de définition des sujets que j'allais couvrir, j'ai découvert une fonctionnalité de C# 11 appelée [`required members`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required) que j'ai naïvement pensé pouvoir utiliser pour signaler ces scénarios, donc j'ai prévu d'ajouter un post montrant comment y parvenir ; cependant, pendant l'investigation/rédaction[^1] du contenu, j'ai réalisé que ce n'était pas l'un des objectifs de cette fonctionnalité et qu'il y avait plusieurs cas particuliers dans lesquels aucun avertissement ne serait émis même si aucun constructeur ne serait invoqué[^2].

L'idée principale était de marquer tous les membres (champs/propriétés) qui seraient initialisés dans les constructeurs comme `required` et d'ajouter l'attribut [`SetsRequiredMembers`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.requiredmemberattribute?view=net-8.0) à ces constructeurs de manière à ce que dans les cas où aucun constructeur (décoré avec `SetRequiredMembersAttribute`) ne serait invoqué, le compilateur émettrait un avertissement/erreur en raison de la non-initialisation de ces membres.

Cette technique fonctionne relativement bien si l'on veut détecter un scénario très problématique : l'instanciation de types valeur avec un constructeur dans lequel tous ses paramètres sont optionnels[^3]. Pour rendre la discussion plus concrète, prenons notre dernier exemple du post précédent et modifions-le comme décrit ci-dessus :

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

Avec ce changement en place, au lieu d'obtenir silencieusement une instance de `S2` initialisée à zéro (contre la valeur attendue de 42 si le constructeur était invoqué), nous obtenons l'erreur suivante :

>error CS9035: Required member 'S2.v' must be set in the object initializer or attribute constructor.

Pas parfait, étant donné que le message sera probablement très confus si l'on s'attend (à tort, mais compréhensible) à ce que le constructeur de `S2` soit invoqué, mais il y a d'autres limitations qui rendent cette approche encore moins viable :

  - Incapacité à détecter les constructeurs non invoqués (au moins) dans les expressions _default_ et les instanciations de tableaux.
  - Même dans les scénarios où cela fonctionnerait, il est impossible de garantir qu'un constructeur sera invoqué (par exemple, si nous changeons le code à la ligne #1 en `new S2() { v = 5 }`, aucun constructeur ne sera invoqué mais aucun avertissement/erreur ne sera émis non plus)

Une alternative plus efficace (si vous déployez votre application en tant qu'application managée plutôt que de la compiler AOT) pour détecter de tels scénarios consiste à vérifier explicitement que les instances de struct ont été initialisées[^4] (soit par un constructeur, soit par d'autres moyens) avant d'accéder à ses membres ; puisque le code est JITé, on peut implémenter cela de manière à ce que les utilisateurs puissent contrôler si la vérification doit être appliquée ou non et avoir très peu (voire aucun) impact sur les performances lorsque l'application est désactivée, comme démontré ci-dessous.

```CSharp
using System.Runtime.CompilerServices;

class Driver
{
    static void Main()
    {

        for(int i = 0; i < 100_000; i++)
        {
            var foo = new Foo(); // Aucun constructeur invoqué... aucun avertissement :(
            Thread.Sleep(100);
            foo.Use();
        }
    }

}

// Le struct peut être déclaré dans un autre assembly également. 
struct Foo
{
    // il est important que le champ soit marqué comme `readonly`
    private static readonly bool _shouldValidate = Environment.GetEnvironmentVariable("VALIDATE_FOO") == "true";

    private int _i;
    private bool _isInitialized;

    public Foo(int v = 1) { _i = v; _isInitialized = true; }

    public void Use()
    {
        Verify();
        System.Console.WriteLine(_i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // Il est important de demander au JIT d'inliner cette méthode pour que l'optimisation soit appliquée.
    private readonly void Verify()
    {
        if (_shouldValidate)
        {
            if (!_isInitialized)
            {
                throw new Exception("Le constructeur de Foo n'a pas été invoqué ; cela peut être dû à une déclaration de tableau ou ...");
            }
        }
    }
}
```

Le code utilise le champ `_shouldValidate` pour contrôler si la vérification de l’initialisation[^4] doit être appliquée ou non (dans ce cas plus spécifiquement, si le constructeur a été exécuté). Notez que ce champ est déclaré comme `static readonly`. Cela est très important car avec cela en place, le JIT sait que, pour une instance de struct donnée, une fois initialisée, la valeur du champ **ne changera jamais**, donc il est libre de traiter `_shouldValidate` comme une constante et de ne pas générer de code pour le vérifier dans le **if** à la ligne #XX ; de plus, dans le cas où il est évalué à **false**, le JIT peut supprimer toute l'instruction `if` (d'où le surcoût proche de zéro mentionné précédemment).

Vous pouvez voir cette *magie* du JIT en action en ouvrant un terminal, en créant une application console avec le code ci-dessus et en exécutant :

```bash
DOTNET_JitDisasm=Use dotnet run -c Release
```

ce qui :

1. sur les systèmes d'exploitation de type `unix`, définit la variable d'environnement `DOTNET_JitDisasm` à `Use`
1. compile l'application en **mode release** (`-c Release`), ce qui est une exigence pour que l'optimisation soit appliquée.
1. exécute l'application.
1. demande au JIT de déverser le code assembleur JITé pour la méthode `Use()` en définissant la variable d'environnement `DOTNET_JitDisasm` en conséquence.

Lorsque vous exécutez cette ligne de commande, vous devriez voir des zéros (0) et du code assembleur ffiché dans le terminal plusieurs fois. Après quelques itérations, vous devriez être capable de repérer du code assembleur ressemblant à celui ci-dessous (assurez-vous de vérifier celui qui contient `Tier1` au lieu de `Tier0`) :

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

Ce qui consiste essentiellement à appeler `System.Console.WriteLine(_i)` et à retourner, sans aucune trace de l’invocation de la méthode `Verify()`.

Vous pouvez également expérimenter cet exemple en l’exécutant comme suit :

```bash
DOTNET_JitDisasm=Use VALIDATE_FOO=true dotnet run -c Release
```

dans ce cas, il lancera une exception (prouvant que l'utilisation d'instances de struct non initialisées sont détectées)

ou

```bash
DOTNET_JitDisasm=Use dotnet run -c Debug
```

dans ce cas, peu importe combien de temps l'application s'exécute, le code assembleur généré pour `Use()` appellera toujours `Verify()` (c'est-à-dire que l'optimisation n'a pas été appliquée car l'application a été compilée en **mode debug**)

Avec cette approche, on peut être sûr qu'aucun code n'utilise d'instances `non initialisées` en exécutant simplement le code avec la variable d'environnement définie à `true` et en observant les exceptions.

Comme toujours, tous les commentaires sont les bienvenus.

Amusez-vous bien !

[^1]: [ici](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwGIATGAMwEs8YACAVQGUBRAfQCVmBFegSU4AiAWABQAAQAMtMQEYAdAMoBDAOZ4IAZwyUwGuQGEIZAIJ4lAGwCeGyhoDcosQCYZo0QDclUWrACOtAF5aMnIlHHMMAAp2GF8ASjtaAHok2lcRT28/J0DaGgB3WhjfSITkpIBvWgA1Cxw6IIBWWgBfWkSU2icPLx9YgGZcgqLYyJkynqzYlCGYQuKxtC6JkVEU2QBOSL85WvN6hMcZLezduphD8WPtgbP9i4crk+m7g8e3ES1cMAwR31EKqIiJRyAwWBxuHxBLQYbRRLDpIM/DhKLASLRqL89vVHkQYOYNHRYfDYWJBpiaudcTA8CQQSSYQzaABtRgwDAaYootEAWRgAFtgDAoBoALpMsl/SIUyhLCkAKzigQAfJT7rl5YliSIEUyUqz2ZzYtyYCQ+YLhWK9UlJQslSq1fVcignI9dTrYQaOVzUabzUKReKPTDbaMKQhck4JPaAqrsQ1aAhHi0Ps5aAIKACJYMM+RpXhfpQlUzAcGEdJrgAiXO0H7QWsWcymyuXBEpkQtIA===) vous pouvez trouver du code de test que j'ai utilisé en explorant ce sujet.

[^2]: Après avoir réalisé cela, j'ai changé le titre du post :).

[^3]: Ce cas particulier est problématique en raison de l'attente que le comportement corresponde au comportement pour les **classes**

[^4]: Note de clarification : Du point de vue du runtime, les structs sont garantis d'être initialisés (en _mettant à zéro tout le struct_) avant d'être utilisés. _Initialisation_ dans le contexte de cette série de posts signifie que tous les champs/propriétés du struct ont été assignés à des valeurs significatives, laissant l'instance dans un état cohérent.