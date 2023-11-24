using System.IO;
using System.Console;

class Test
{
	public static void Main(string[] args)
	{
		if (args.Length == 0)
		{
			WriteLine("arquivo?");
			return;
		}
		
		using(var arq = File.OpenText(args[0]))
		{
			while ( (var line = arq.ReadLine()) != null )
			{
				WriteLine("> {0}", line);
			}
		}		
	}
}