static class StringExtensions
{
	public static extension bool ContemAdriano
	{ 
		get 
		{
			return value.Contains("Adriano"); 	// value representa a string.
												// Em nosso exemplo de chamada 
												// value representa o parâmetro 's'
		} 
	}
}

class Exemplo
{
	public bool FazAlgo(string s)
	{
		return s.ContemAdriano;
	}
}