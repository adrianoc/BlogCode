<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/brazil_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;"><a href="https://programing-fun.blogspot.com/2025/03/structs.9.pt.html">Leia este post em Português.</a></p>

<p><img src="https://github.com/adrianoc/BlogCode/blob/041acbc54fda6b4c970bebfbb89da899be6e2190/StructCtors/images/united_kingdom_glossy_wave_icon_64.png?raw=true" style="width:32px;height:32px;vertical-align: middle;border: 0px; padding: 0px; box-shadow: none;"><a href="https://programing-fun.blogspot.com/2025/03/structs.9.en.html">Read this post in English.</a></p>

<script>
populateToc("https://raw.githubusercontent.com/adrianoc/BlogCode/refs/heads/main/StructCtors/toc.fr.js", 'struct-series-toc-9-fr');
</script>
<p id="struct-series-toc-9-fr">

Dans les articles précédents de cette série, en explorant le comportement des constructeurs dans les `structs` en C#, nous avons découvert que, dans certains scénarios, ces constructeurs ne sont pas exécutés (malgré une syntaxe qui pourrait nous faire croire le contraire).  

Être conscient de ce comportement est essentiel, et une attention particulière doit être portée lors de la définition d'API publiques (surtout dans les bibliothèques en raison de leur portée potentielle) afin d'éviter de créer des API contre-intuitives et/ou sujettes aux erreurs, car les développeurs pourraient facilement tomber dans le piège de l'utilisation de membres non initialisés[^1].  

Cela dit, ce comportement n'est pas nouveau ; les problèmes évoqués dans les articles [#5](https://programing-fun.blogspot.com/2023/12/structs-in-c-are-fun-part-59-other.html), [#6](https://programing-fun.blogspot.com/2024/01/structs-in-c-are-fun-part-69-struct.html) et [#7](https://programing-fun.blogspot.com/2025/01/structs.7.fr.html) existent depuis les premières versions du langage. Cependant, l'introduction, en C# 10, de la possibilité de définir des constructeurs **sans paramètre** dans les `structs` ajoute une couche de complexité supplémentaire et augmente les chances que les développeurs soient exposés à de tels types[^2].  

Ainsi, si vous consommez des types `struct`, soyez vigilant afin de ne pas utiliser d'instances non initialisées[^1]. D'un autre côté, faites particulièrement attention lorsque vous définissez de nouveaux types `struct`, en particulier si vous ajoutez des **constructeurs sans paramètres** ou un **constructeur dont tous les paramètres sont optionnels**.  

Avec cet article, nous atteignons la fin de cette série; J'espère que vous l'avez apprécié et, peut-être appris quelque chose de nouveau au cours de ce voyage et que êtes maintenant mieux préparés à la fois pour utiliser et exposer vos propes structures.

Comme toujours, tout retour est bienvenu.  

Amusez-vous bien !  

[^1]: **Note de clarification** : Du point de vue du runtime, les `structs` sont garantis d’être initialisés (par une mise à zéro complète de la structure) avant d’être utilisés. Le terme _initialisation_ dans cette série d’articles signifie que **tous les champs/propriétés du `struct` ont reçu des valeurs significatives**, laissant ainsi l’instance dans un état cohérent.

[^2]: Avant C# 10, la seule façon pour un développeur d'être exposé à de tels types était de consommer des assemblages compilés avec un langage prenant en charge cette fonctionnalité, comme IL ; personnellement, je n’ai jamais rencontré un seul exemple de ce genre.  