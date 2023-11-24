void M()
{
	int valor;
	if(Int32.TryParse("42", out valor))
		return valor;

	return -1;
}