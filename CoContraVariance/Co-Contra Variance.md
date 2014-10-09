{/
  +git:GitContent -> HtmlEncode
  +sh:SyntaxHighlighter
  +nb:NiceBox
/}

Hi

Every now and then I find myself thinking about 2 concepts that has "just" been "extended" in C# language version 4.0 ;): co / contravariance. The interesting fact is that it always seems like my brain is going to blow :)

PS: I used the words "just" and "extended" in quotes because version 4.0 of the language has been released in April/2010 (accordingly to Wikipedia) and C# supported covariance (in some way or another) since version 1.0!

So, in order to try to assimilate the concept, once and for all I decided to do what I believe to be the most effective way to learn, i.e, to try to explain it to others ;) so, if you already master the subject, go away, do something else :) (this will be an informal discussion about these topics. If you want a more formal one, please see [this Wikipedia page][1] and possible follow the links).

Basically, as the Wikipedia article puts very well (IMO) variance refers to how type inheritance affects the relationship of more complex language constructs (such arrays of derived types instead of arrays of the base type, functions returning a derived type instead of a function return the base type, etc)

Take a look in the following piece of C# code:

{| git=ArrayCovariance.cs@co-contra-reasoning sh.toolbar=false sh.highlight=[12]  |}

Pretty simple, huh?

In line 12 we create an array of Derived objects and initialize it with 2 objects; then on line 13 we assign this array to a variable declared as an 'array of Base' objects and it just works, after all, **Derived** inherits from Base, and developers expects such assignment to work.

But don't get too excited about array covariance yet because... it is broken

The problem is that arrays in C# are covariant, *pero no mucho*. Suppose that in the next version of the program, you were tasked to, after printing the contents to console, replace the first element of the array with an instance of the **Base** class, so you come up with the following code:

namespace CoContraVariance 
{
  class Base { } 
  class Derived : Base { } 

  class Program 
  {
    static void Main(string[] args)
    { 
      Derived[] derivedArray = new [] { new Derived(), new Derived() }; 
      Base[] baseArray = derivedArray; 

      foreach (var item in baseArray) { System.Console.WriteLine(item); }

      baseArray[0] = new Base();
    }
  }
} 

Piece of cake, if you don't mind an exception being thrown at line 21. Why? Well, because since arrays in C# are covariant, but pero no mucho, you can handle an array of a derived type as an array of a base class (like our sample) when you are reading from it, but not when you are *assigning to it*! It is even worse than that: every assignment need to be checked for type correctness so we are paying an extra performance price whenever we assign to any position of the array.

Another C# construct that supports co/contravariance since version 2.0 of the language is "*method group to delegate conversions*". As an example, take a look in the following code:

void Foo(Func<Base> f) 
{  
} 

Derived Bar()
{  
} 

Even though Foo is declared as taking a function that returns a reference to a Base object, it is perfectly valid to call it as:

Foo(Bar); 

Why? Well, whatever value "f" (in this case *Bar()* method) returns to Foo it will be either an instance of Derived or of a sub class of Derived. In any case this object "is an" instance of Base, which Foo() is prepared to handle.

The converse is also valid for parameters (in this case it is called contravariance):

void Foo(Action<Derived> f)
{  
}

void Bar(Base base)
{  
}

Foo(Bar);

Note that, even though conversions from method group to delegates are co/contra variant since C# version 2.0, generic delegates are not! So, the following code is invalid on C# versions < 4.0

Action<Derived> ad = Bar; // Ok, valid, method group to delegate conversion (contravariance)
Action<Base> ac = Bar;
Action<Derived> ad_error = ac; // Error.

Ok, now, as every good C# dev is aware, arrays implements the IEnumerable<T> interface, so intuitively developers expect that the following change should be supported:

{| git=ArrayCovariance.cs@co-contra-generic sh.toolbar=false sh.highlight=[12]  |}

But, if you try to compile this sample against C# version < 4.0 you'll get an error in line 24 similar to: 

<div class="out-shadow">Error CS0266: Cannot implicitly convert type 'IEnumerable&lt;Derived&gt;' to 'IEnumerable&lt;Base&gt;'. An explicit conversion exists (are you missing a cast?)</div>

This happens because in C# language, prior to version 4.0, generic interfaces were invariant, i.e, given an interface Itf&lt;T&gt; and types Base and Derived (Derived inheriting from Base) Itf&lt;Derived&gt; had no inheritance relationship with Itf&lt;Base&gt; whatsoever!

In C# version 4.0 the language designers introduced co/contra variance for generic interfaces and generic delegates! The first implication for us, is that, if we try to compile our previous sample (the one in which we play with IEnumerable&lt;T&gt;) with such C# compiler version it works! (that happens because MS annotated IEnumerable&lt;T&gt; as covariant). 

The second implication for us is that now we can mark our very own interfaces as such!

<pre class="brush: csharp; highlight:[17]">
namespace CoContraVariance 
{ 
  interface IFoo&lt;out T&gt; 
  { 
    T GetValue();
  } 

  class Base { } 
  class Derived : Base { } 

  class Program 
  { 
    static void Main(string[] args) 
    { 
      IFoo&lt;Derived&gt; derivedItf = null; 
      IFoo&lt;Base&gt; baseItf = derivedItf; 
    } 
  } 
} 
</pre>

The first thing that pops out our eyes is the word "out" (no pun intended) besides the generic parameter "T" and that is the way we tell the compiler that the interface IFoo is covariant in T (if you remove this "marker" line 24 becames invalid again). 

<div class="out-shadow">
Just in case it is not clear yet, covariant type parameters may only appear as the return type of methods / properties; if you try to define parameters of such types the compiler will kindly remind you that this is not valid ;)
</div>

The other language construct, called contravariance, allows us to handle generic interfaces / delegates of a base type as interface/delegate to a more derived one (IMO, contravariance is harder to grasp since it looks like it goes against the "normal" inheritance rules).

<pre class="brush: csharp">
namespace CoContraVariance 
{ 
  interface IFoo&lt;in T&gt; 
  { 
    void DoIt(T value);
  } 

  class Base { } 
  class Derived : Base { }   
  class MoreDerived : Derived { } 

  class Program 
  { 
    static void Main(string[] args) 
    { 
      IFoo&lt;Base&gt; baseItf = null; 
      IFoo&lt;Derived&gt; derivedItf = baseItf;

      derivedItf.DoIt( new Derived() );
      derivedItf.DoIt( new MoreDerived() );
    }
  } 
}
</pre>

In this sample, when a call to derivedItf.DoIt() is made we'll be calling baseItf.DoIt(). Since the compiler will enforce that anything we pass to the former is either an instance of Derived (or of some class that inherits from it) and that Derived "is a" Base, the actual method called will get an instance of Base (which it expects)!

If you want to read more about it I recommend [this excelent][2], in deph, series of posts</a> and also the following links:

http://msdn.microsoft.com/en-us/library/ee207183.aspx

http://msdn.microsoft.com/en-us/library/dd799517(v=vs.110).aspx 

http://en.wikipedia.org/wiki/Covariance_and_contravariance_(computer_science)

Happy codding.

[1]: http://en.wikipedia.org/wiki/Covariance_and_contravariance_(computer_science)
[2]: http://blogs.msdn.com/b/ericlippert/archive/tags/covariance+and+contravariance/