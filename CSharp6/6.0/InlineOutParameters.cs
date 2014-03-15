void M()
{
	return Int32.TryParse("42", out int valor) ? valor : -1;
}