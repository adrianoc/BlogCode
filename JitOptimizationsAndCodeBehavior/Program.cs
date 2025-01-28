using System.Text;

var o = new Test();
Console.WriteLine(o.M(1));

class Test
{
    private static readonly CompositeFormat _format = CompositeFormat.Parse("Sleeping for {0}ms");
    private static int Year = System.DateTime.Now.Year;

    internal int M(int n)
    {
        if (n % 10000 == 0)
        {
            // https://github.com/dotnet/BenchmarkDotNet/issues/1993#issuecomment-1117419356 
            // The comment above says that 100ms of no new methods called is enough to trigger counting
            // number of times M() is called and after 30 calls it should be promoted to tier 1
            // however, in my machine this value is sensitive to as how many times we sleep.
            // Sleeping for 100ms every 10_000 iterations is enough on my linux machine, but
            // on a Windows one (with slightly different .NET runtime versions) 150ms was the
            // minimum.
            const ushort SleepTime = 150;
            Console.WriteLine(string.Format(null, _format, SleepTime));
            Thread.Sleep(SleepTime);
        }

        // Simulates an always false check that is harder for the compiler / JIT to detect and
        // deem that as "dead code"  and eliminate the *if*
        if (Year == 1971)
            return 0;
            
        return M(n - 1);
    } 
}