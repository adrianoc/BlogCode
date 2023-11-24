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
    }
  }
}