namespace Lib1;

public class MyClass
{
    public string DoIt(int i, int j)
    {
        var s = "";
        var r = 0;
        if (i % 2 == 0)
            s = "1";

        if (j > 5)
            s = "2";

        return s;
    }
}
