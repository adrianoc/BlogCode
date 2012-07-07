using System;
using SampleClassLibrary;

namespace UsageSample
{
	class Program
	{
		static void Main(string[] args)
		{
			UsefulType<Func<int>, string, int> u = new UsefulType<Func<int>, string, int>(null, "test", 1);
		}
	}
}
