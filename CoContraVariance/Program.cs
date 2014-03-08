using System;
using System.Collections;
using System.Collections.Generic;

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

			foreach (var item in baseArray)
			{
				Console.WriteLine(item);
			}

			IEnumerable<Derived> deriveds = derivedArray;
			IEnumerable<Base> bases = deriveds;
		}
	}
}
