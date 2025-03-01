<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/brazil_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;"><a href="https://programing-fun.blogspot.com/2025/01/structs.8.pt.html">Leia este post em Português.</a></p>

<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/united_kingdom_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;"><a href="https://programing-fun.blogspot.com/2025/01/structs.8.en.html">Read this post in English.</a></p>

<script>
populateToc("https://raw.githubusercontent.com/adrianoc/BlogCode/refs/heads/main/StructCtors/toc.fr.js", 'struct-series-toc');
</script>
<p id="struct-series-toc">

Dans un article précédent, nous avons examiné l'utilisation de valeurs de paramètres par défaut **dans les constructeurs de structures** ; cette fois, nous allons examiner rapidement ce qui se passe lorsque nous avons des paramètres de structure avec des valeurs par défaut. Alors, sans plus tarder, commençons par la structure suivante[^1] :

```csharp
struct S
{
    public int v = 42;
    public S(string s) { v = 13; }
}
```

et l'utilisation suivante :

```csharp
M();

void M(S s = new S()) => Console.WriteLine(s.v);
```

Quelle sortie attendez-vous à obtenir ?

Si vous avez suivi cette série d'articles, vous savez probablement qu'il s'agit d'une question piège et que ni *42* ni *13* ne sont les bonnes réponses, car cela nécessiterait que l' *initialiseur de champ* et/ou le constructeur soient exécutés, mais l'expression `new S()` utilisée comme `valeur de paramètre par défaut` n'implique pas un appel de constructeur (après tout, il n'y a pas de constructeur sans paramètre et si vous êtes tenté d'en ajouter un, le compilateur émettra une erreur car en présence d'un tel constructeur, l'expression en question ne représente plus une constante de compilation, d'où elle ne peut pas être utilisée comme valeur de paramètre par défaut).

Lorsqu'il est exécuté, cet extrait de code affichera `0` car le compilateur initialise simplement la mémoire complète utilisée pour stocker l'instance de structure avec des zéros (`0`). Par contre, si vous appelez la méthode `M()` comme suit[^2]

```csharp
M(new S("Foo"));
```

dans cette cas le compilateur émettra du code pour exécuter le constructeur et l'initialiseur de champ et `13` sera affiché, mais cela n'a plus rien à voir avec la `valeur de paramètre par défaut`.

Et avec cela, nous avons exploré le dernier comportement de structure `pas si intuitif` que nous avions l'intention de couvrir ; le prochain article sera la conclusion de cette série.

Comme toujours, tous les commentaires sont les bienvenus.

Amusez-vous bien !

---
[^1] : Notez que la prise en charge des initialiseurs de champs dans les structures a été introduite dans C# 10.

[^2] : Le contenu de la chaîne n'est pas important dans ce contexte. Tout ce qui compte, c'est qu'un constructeur est appelé.