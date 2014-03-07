using System;
using System.IO;

namespace TestFileLocked
{
	class Program
	{
		static void Main()
		{
			string fileName = @"c:\temp\TestAdriano.txt";
			using(new FileStream(fileName, FileMode.OpenOrCreate))
			{
				Console.WriteLine("{0} open.", fileName);
				Console.WriteLine();
				Console.WriteLine("Press any key to close the file and exit.");
				Console.ReadKey();
			}
		}
	}
}
