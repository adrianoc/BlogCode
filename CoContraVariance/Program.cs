using System;

namespace CoContraVariance
{
	interface IFoo<out T, in I>
	{
		T GetValue();
		T AsProp { get; }
		I AsPropI { set; }
	}

	class Base { }

	class Derived : Base { }

	class Program
	{
		static void Main(string[] args)
		{
			//IFoo<Derived> derivedItf = null;
			//IFoo<Base> baseItf = derivedItf;
			
			Action<Derived> ad = Foo;
			
			Action<Base> ab = Foo;
			Action<Derived> ad2 = ab;
		}

		private static void Foo(Base b)
		{
			
		}
	}
}
