public interface IFoo
{
	void DoIt();

	string Name { get; }
}

class Exemplo
{
	public void FazAlgo()
	{
		var itfImpl = new IFoo
		{
			void DoIt() { }

			string Name { get { return "name"; } }
		}
	}

	System.Console.WriteLine(itfImpl.Name);
}