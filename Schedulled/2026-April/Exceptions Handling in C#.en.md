# TOC
- Quick review of exceptions in .NET (only .NET, not `.NET Framework`)
- 1st & 2nd chance exceptions
- Exceptions that cannot/should not be caught
    - [StackOverflowException](https://learn.microsoft.com/en-us/dotnet/api/system.stackoverflowexception?view=net-10.0) (cannot)
    - [OutOfMemoryException](https://learn.microsoft.com/en-us/dotnet/api/system.outofmemoryexception?view=net-10.0) (should not)
- Common pitfalls
    - Correct way to re-throw exceptions (ExceptionDispatcherInfo, `throw` and `throw ex`). [See](https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions#capture-and-rethrow-exceptions-properly)
    - Swallowing exceptions
    - Exceptions in static constructors

- Improving exception stack traces
    - https://github.com/benaadams/Ben.Demystifier (original, seems abandoned)
    - https://github.com/getsentry/Ben.Demystifier (fork, seems to be more updated)

- Exceptions in async code ??

# Non Goals

-  Get into the discussion whether exceptions is a good idea or not

## TypeInitializationException

```C#
Print();

void Print()
{
    Console.WriteLine("Computing.");
    var f = new Foo();
    Console.WriteLine(f);
    Foo.Do();
    Console.WriteLine($"{Foo.Name}");
}

class Foo
{
    public static int value = Compute(1);

    public static string Name = "Fooooo";

    public static void Do() {}

    private static int Compute(int n)
    {
        if (n < 2)
        {
            throw new ArgumentException($"Parameter ({nameof(n)}) should be greater than 2 but it {n}");
        }

        return n % 2;
    }
}
```
Generates

```
Unhandled exception. System.TypeInitializationException: The type initializer for 'Foo' threw an exception.
 ---> System.ArgumentException: Paramenter (n) should be greater than 2 but it 1
   at Foo.Compute(Int32 n) in /tmp/exception/Program.cs:line 24
   at Foo..cctor() in /tmp/exception/Program.cs:line 14
   --- End of inner exception stack trace ---
   at Program.<<Main>$>g__Print|0_0() in /tmp/exception/Program.cs:line 9
   at Program.<Main>$(String[] args) in /tmp/exception/Program.cs:line 1
```

but it claims that line 9 in method Print() (`Console.WriteLine($"{Foo.Name}");`) triggered line 14 in class Foo (`public static int value = Compute(1);`) which threw an exception but line 9 is accessing field `Name` and apparently it has no connection with the field `value` (which is being initialized at line 14).

The explanation is that when first access to static field happens, all static fields are initialized.