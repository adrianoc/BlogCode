class Example
{
	public void M()
	{
		try 
		{ 
		} 
		catch(MyException me) 
		{ 
			if (me.SomeProp == some_value) 
			{ 
			// trata a exceção aqui 
			} 
			else 
			{ 
				throw; 
			} 
		}
	}
}