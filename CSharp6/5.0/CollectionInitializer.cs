using System.Collections;

class Coll : IEnumerable
{
	public void Add(string s)
	{
		System.Console.WriteLine(s);
	}

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