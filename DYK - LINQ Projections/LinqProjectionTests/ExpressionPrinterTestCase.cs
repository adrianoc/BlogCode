using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace LinqProjectionTests
{
	[TestFixture]
	public class ExpressionPrinterTestCase
	{
		[Test]
		public void TestSimpleField()
		{
			AssertExpressionRepresentation("P[i:Item]=>P[i:Item].Name", (Item i) => i.Name);
			AssertExpressionRepresentation("P[i:Item]=><P[i:Item].Value[Add]10>", (Item i) => i.Value + 10);
		}

		[Test]
		public void TestExpressionsWithAnonymousTypes()
		{
			AssertExpressionRepresentation("P[str:String]P[value:Int32]=>new{P[str:String]P[value:Int32]}", (string str, int value) => new { str, value });
			AssertExpressionRepresentation("P[str:String]P[value:Int32]=>new{P[str:String]<P[value:Int32][Add]42>}", (string str, int value) => new { str, Value = value + 42});
		}

		private void AssertExpressionRepresentation<S,T>(string expectedRepresentation, Expression<Func<S,T>> actualExpression)
		{
			Assert.AreEqual(expectedRepresentation, new ExpressionPrinter(actualExpression).ToString());
		}
		
		private void AssertExpressionRepresentation<P1, P2, T>(string expectedRepresentation, Expression<Func<P1, P2, T>> actualExpression)
		{
			Assert.AreEqual(expectedRepresentation, new ExpressionPrinter(actualExpression).ToString());
		}
	}
}
