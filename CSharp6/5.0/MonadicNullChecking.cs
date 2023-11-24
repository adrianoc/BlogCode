class Exemplo
{
	public int M(IList<string> ss)
	{
		if (ss != null)
		{
		 	var primeiraOuNulo = ss.FirstOrDefault();
		 	if (primeiraOuNulo != null)
		 		return primeiraOuNulo.Length;
		}
		return -1;
	}
}