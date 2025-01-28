
    If struct does not have a parameterless ctor one can use the syntax TypeName t = new()
    Otherwise default need to be used
    Regardless, the effect is the same, no ctor is invoked and struct is zerod out

M();
M(new S2());

void M(S2 s = default) => System.Console.WriteLine(s.v);

//void M2(S2 s = new()) => System.Console.WriteLine(s.v);
//this produces the error:
//error CS1736: Default parameter value for 's' must be a compile-time constant
//that is because S2 has a parameterless ctor

struct S2 
{ 
    public int v = 71;
    public S2()  => v = 42;
}

prints 0, 42