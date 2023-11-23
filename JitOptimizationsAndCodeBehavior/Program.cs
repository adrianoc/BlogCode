// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

var o = new Test();
Console.WriteLine(o.M(10));

class Test
{
    private static readonly CompositeFormat _format = CompositeFormat.Parse("Sleeping for {0}ms");

    internal int M(int n)
    {
        if (n == 0)
        {
            // https://github.com/dotnet/BenchmarkDotNet/issues/1993#issuecomment-1117419356 
            // The comment above says that 100ms of no new methods called is enough to trigger counting
            // number of times M() is called and after 30 calls it should be promoted to tier 1
            // however, in my machine, a sleep of 180ms is the minimum.
            const ushort SleepTime = 180;
            //System.Console.WriteLine($"Sleeping for {SleepTime}ms");
            Console.WriteLine(string.Format(null, _format, SleepTime));
            Thread.Sleep(SleepTime);
        }

        return N(n - 1);
    }

    internal int N(int n)
    {
        return M(n -1);
    }
}