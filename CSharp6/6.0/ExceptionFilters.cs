class Example
{
	public static void Main()
	{
		try 
		{ 
		} 
		catch(MyException me => me.SomeProp == 42)
		{ 
			// trata a exceção aqui 
		}
	}
}

class MyException : System.Exception
{
	public int SomeProp;
}