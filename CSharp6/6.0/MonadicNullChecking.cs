class Exemplo1
{
	public int M(IList<string> ss)
	{
		return ss?.FirstOrDefault()?.Length ?? -1;
	}
}
