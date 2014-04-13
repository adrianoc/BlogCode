class Example
{
	public static void Main()
	{
		try 
		{ 
		} 
		catch(MyException me) if (CheckException(me))
		{ 
			// trata a exceção aqui 
		}
	}

	private static bool CheckException(MyException ex)
	{
		return ex.SomeProp == 42;
	}
}

class MyException : System.Exception
{
	public int SomeProp;
}