Some time ago I was investigating a bug in one of my programs and I stumbled across an odd behavior.

- Run in release
  - DOTNET_JitDisasm=M dotnet run -c Release
  - Runs forever, since the `recursion` is turned by the JIT into a `tail call recurse` which means
    the recursion is converted to the equivalent loop.
- Run in *debug* mode
  - a stack overflow happens

- This may make investigating bugs in your code harder (you get an infinity loop instead of a stack overflow)
  