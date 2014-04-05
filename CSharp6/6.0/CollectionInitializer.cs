using System.Collections;


static class Extensoes
{
	public static void Add(this Coll coll, string s)
	{
		System.Console.WriteLine(s);
	}
}

class Coll : IEnumerable
{
	public IEnumerator GetEnumerator() { return null; }
}

class Test
{
	public static void Main(string[] args)
	{
		var x = new Coll() 
		{ 
			"foo",
			"bar",
			"baz" 
		};
	}
}