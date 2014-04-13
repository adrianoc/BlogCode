class Example (private int value, string n)
{
	private string name = n;

	public static void Main(string[] args)
	{
		new Example(42, "foo").Print();
	}

	private void Print()
	{
		System.Console.WriteLine("{0} = {1}", name, value);
	}
}