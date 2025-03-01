## Compiler defensive copies

1. readonly fields
2. `in` parameters
3.  locals `ref readonly`

```C#
using System;
public class C 
{
    private readonly Foo roFoo;
    private Foo mutableFoo;
    
    void TestMutableField()
    {
        ref readonly Foo r = ref mutableFoo;
        
        mutableFoo.MutatingMethod(); // no copy
        r.MutatingMethod(); // !copy
    }
    
    public void M(in Foo inFoo) 
    {
        inFoo.ReadOnlyMethod(); // no copy
        roFoo.ReadOnlyMethod(); // no copy
        
        inFoo.MutatingMethod(); // !copy
        roFoo.MutatingMethod(); // !copy
        
        Console.WriteLine(inFoo.RoProperty); // no copy
        Console.WriteLine(roFoo.RoProperty); // no copy
        
        Console.WriteLine(inFoo.MutatingProperty); // !copy
        Console.WriteLine(roFoo.MutatingProperty); // !copy
       
    }
}

public struct Foo
{
    public readonly void ReadOnlyMethod() {}
    
    public void MutatingMethod() {}
    
    public readonly int RoProperty  => _prop;
    public int MutatingProperty  => _prop;
    
    int  _prop;
}
```