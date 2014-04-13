using System.Collections.Generic;
using System.Console;

class Example
{
	private IDictionary<string, int> data = new Dictionary<string, int>();

	public int this[string i] 
	{ 
		get { return data[i]; }
		set { data[i] = value ; }
	}

	public static void Main(string[] args)
	{
		var dic2 = new Dictionary<int, string>() 
		{ 
			{0, "nada" },
			{1, "uno" }
		};

		var dic = new Dictionary<int, string>() { [0] = "nada", [1] = "uno" };			
		var inst = new Example { $zero = 0, $one = 1, $two = 2 };

		WriteLine("Hello World from roslyn! {0} {1}", inst.$zero, inst["zero"]);
	}
}