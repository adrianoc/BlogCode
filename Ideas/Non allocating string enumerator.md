# Non allocating string splitting enumerator

```CSharp
using System;

// TODO: Benchmark

const string data = "ADRI,B,C,TESTE,OOI";

Naive(data);

Console.WriteLine("-----");

NonAllocating(data);

void Naive(string data)
{
    var found = data.IndexOf(',');
    var start = 0;
    
    while (found != -1)
    {
        Console.WriteLine(data.Substring(start, found - start));
        start = found + 1;
        found = data.IndexOf(',', start);
    }
    
    Console.WriteLine(data.Substring(start));
}

void NonAllocating(ReadOnlySpan<char> data)
{
    var e1 = new Enum1(data);
    foreach(var span in e1)
    {
        Console.Out.WriteLine(span);
    }
}

ref struct Enum1
{
        ReadOnlySpan<char> _data;
        int _last = -1;
        int _current = -1;
        
        public Enum1(ReadOnlySpan<char> data)
        {
            _data = data;
        }
        
        public Enum1 GetEnumerator() => this;
        
        public ReadOnlySpan<char> Current
        {
            get => _data.Slice(_last, _current - _last);
        }

        public bool MoveNext()
        {
            _last = _current + 1;
            if (_last < _data.Length)
            {
                var next = _data.Slice(_last).IndexOf(',');
                _current = next != -1 ?_last + next : _data.Length;

                return true;
            }
            
            return false;
        }
}
```