Some time ago I was investigating a bug in some C# code when I stumbled across an odd behavior: a bug report claimed that a code similar to the one presented below would cause a hang, and that that was 100% reproducible:

```C#
int M(int n)
{
  if (SomeMethod(n))
  {
      // some code that is not relevant.
      return;
  }

  return M(n - 1);
}
```
After poking around the code, my best hypotheses was that somehow `SomeMethod()` was never returning; the problem with that hypotheses was that that method was really simple and I could not see how it could hang.

With that option almost ruled out my next best explanation would be that, somehow, `SomeMethod()`  never returned `true` in which case the code would simply call itself in an endless recursion, but that presented another problem: if that was the case, at some point the program would end up running out of stack space and would throw a `StackOverflowException` which, according to the bug report, did not happen.

With no other hypotheses left I finally decided to reproduce the bug by executing the steps as described in the bug report but to my surprise, instead of a hang, the code threw a `StackOverflowException` inside method `M()` indicating that, indeed `SomeMethod()` was never returning `true`. At that point I was really confused: What could be causing the different behavior? Was the stack overflow caused by the same problem or could it be something different? 

To make things worse, the program was not running on `.NET Framework`; instead it was running on Mono runtime which itself is not 100% identical to .NET runtime!
After some head scratching I decided to run the program exactly like the bug reporter; you see, as a developer I usually run the product built locally on my machine instead of installing it from the officially released binaries so I installed the product from an official installer and run the reproduction steps.

This time, to my surprise (again), the program froze. Now I was even more confused: running the program built locally (from exactly the same git revision as reported in the bug) was causing a `StackOverflowException` whereas when run the program installed from an installer it would simply hang (as described in the bug). After experimenting with a couple of possibilities I realized that one big difference between the two versions was that the one I built locally was built in *debug mode* while the one included in installers was built in *release mode* so I decided to give it a try by recompiling my local version in *release mode* and running the reproducible steps; this time (for the third time) to my surprise, the program froze so I finally was able to reproduce the bug (and with the program compiled in *release mode* it was 100% reproducible).

Next step was to understand: i) why the program was hanging and ii) what was causing the difference in behavior between *debug* and *release* modes. From the user point of view, **i** was more important and whence should be my priority but from a programmer point of view **2** was way more interesting :). Nevertheless I refrained myself from spending a lot of time trying to explain what was causing **2** and, instead, debugged the program and realized that indeed there was a bug in **SomeMethod()** which prevent it from ever return **true**.

After that I implemented a fix (and added a unit test, of course ;)) and closed the bug after verifying that my fix was effective but being a developer I was not 100% happy with the outcome: given that the root cause of the problem was `SomeMethod()` never returning `true` I would expect to 



**Experiment**

- Run in release
  - DOTNET_JitDisasm=M dotnet run -c Release
  - Runs forever, since the `recursion` is converted into a `tail call recurse` by the JIT, i.e,
    the recursion is converted to the equivalent loop.
- Run in *debug* mode
  - a stack overflow happens

- This may make investigating bugs in your code harder (you get an infinite loop instead of a stack overflow)
