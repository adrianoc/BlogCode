using Lib1;
using NUnit.Framework;

namespace Lib1Test;

[TestFixture]
public class Class1
{
    [Test]
    public void Test1()
    {
        var x = new MyClass();
        Assert.That(x.DoIt(1, 4), Is.EqualTo(""));
    }
    
    [Test]
    public void Test2()
    {
        var x = new MyClass();
        Assert.That(x.DoIt(2, 4), Is.EqualTo("1"));
    }
    
    [Test]
    public void Test3()
    {
        var x = new MyClass();
        Assert.That(x.DoIt(1, 10), Is.EqualTo("2"));
    }
    // [Test]
    // public void Test4()
    // {
    //     var x = new MyClass();
    //     Assert.That(x.DoIt(2, 10), Is.EqualTo("12"));
    // }    
}
