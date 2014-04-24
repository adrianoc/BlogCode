class Test(string name)
{
	public string Name { get; } = name;

	public static void Main(string[] args)
	{
		System.Console.WriteLine(new Test(args[0]).Name);
	}
}