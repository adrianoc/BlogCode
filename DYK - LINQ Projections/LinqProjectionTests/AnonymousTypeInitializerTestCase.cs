using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Db4objects.Db4o.Linq.Expressions;
using NUnit.Framework;

namespace LinqProjectionTests
{
	[TestFixture]
	public class AnonymousTypeInitializerTestCase
	{
		[Test]
		public void TestSimpleProperty()
		{
			AssertInitializer((string Name) => Name, InitializerExpressionFor((Item item) => item.Name));
		}

		[Test]
		public void TestAddOperationOnSimpleProperty()
		{
			AssertInitializer((int Value) => Value + 10, InitializerExpressionFor((Item item) => item.Value + 10));
		}

		[Test]
		public void TestNewOperator()
		{
			AssertInitializer((string Name) => new { Name }, InitializerExpressionFor((Item item) => new { item.Name }));
			AssertInitializer((string Name, int Value) => new { Name, Value }, InitializerExpressionFor((Item item) => new { item.Name, item.Value }));
		}

		[Test]
		public void TestExpressionsWithConstants()
		{
			AssertInitializer((string Name, int Value) => new { Name, Value = Value + 10}, InitializerExpressionFor((Item item) => new { item.Name, Value = item.Value + 10}));
			AssertInitializer((string Name, int Value) => new { Name = Name + " Doe", Value}, InitializerExpressionFor((Item item) => new { Name = item.Name + " Doe", item.Value }));
		}

		[Test]
		public void TestNotOptimizableExpressions()
		{
			AssertInitializerNotOptimizable((Item item) => item);
			AssertInitializerNotOptimizable((Item item) => new { item });
			AssertInitializerNotOptimizable((Item item) => new { item.Name });
			AssertInitializerNotOptimizable((Item item) => new { item.Name, item.Value });
		}

		private Expression InitializerExpressionFor<S, T>(Expression<Func<S, T>> expression)
		{
			return new ParameterExpressionExpanderVisitor(expression).Expand();
		}

		private void AssertInitializerNotOptimizable<S, T>(Expression<Func<S, T>> expression)
		{
			AssertInitializer(expression, InitializerExpressionFor(expression));
		}

		private void AssertInitializer<P1, P2, T>(Expression<Func<P1, P2, T>> expected, Expression actual)
		{
			Assert.AreEqual(AsString(expected), AsString(actual));
		}

		private void AssertInitializer<S, T>(Expression<Func<S, T>> expected, Expression actual)
		{
			Assert.AreEqual(AsString(expected), AsString(actual));
		}

		private string AsString(Expression expression)
		{
			return new ExpressionPrinter(expression).ToString();
		}
	}

	internal class ParameterExpressionExpanderVisitor : ExpressionTransformer
	{
		private readonly Expression _expression;
		private IDictionary<string, ParameterExpression> _args;

		public ParameterExpressionExpanderVisitor(Expression expression)
		{
			_expression = expression;
		}

		protected override Expression VisitLambda(LambdaExpression lambda)
		{
			_args = CreateParameters();
			Expression newBody = Visit(lambda.Body);
			return Expression.Lambda(newBody, _args.Values.ToArray());
		}

		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			ParameterExpression parameter = m.Expression as ParameterExpression;
			if (parameter != null)
			{
				return _args[m.Member.Name];
			}

			return m;
		}

		private IDictionary<string, ParameterExpression> CreateParameters()
		{
			var args = new Dictionary<string, ParameterExpression>();
			List<MemberInfo> members = CollectAccessedFields(_expression);
			foreach (var member in members)
			{
				args[member.Name] = Expression.Parameter(TypeOf(member), member.Name);
			}

			return args;
		}

		private Type TypeOf(MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Field)
			{
				return ((FieldInfo) member).FieldType;
			}

			if (member.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo) member).PropertyType;
			}

			throw new InvalidOperationException();
		}

		private List<MemberInfo> CollectAccessedFields(Expression expression)
		{
			FieldAccessCollector collector = new FieldAccessCollector(expression);
			return collector.Collect();
		}

		public Expression Expand()
		{
			return Visit(_expression);
		}
	}

	internal class FieldAccessCollector : ExpressionTransformer
	{
		private readonly Expression _expression;
		private List<MemberInfo> _fieldList;
		private ParameterExpression _parameter;

		public FieldAccessCollector(Expression expression)
		{
			_expression = expression;
		}

		public List<MemberInfo> Collect()
		{
			_fieldList = new List<MemberInfo>();
			Visit(_expression);

			return _fieldList;
		}

		protected override Expression VisitParameter(ParameterExpression p)
		{
			if (_parameter != null && _parameter != p)
			{
				throw new InvalidOperationException();
			}
			_parameter = p;
			return p;
		}

		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			Visit(m.Expression);
			if (m.Expression == _parameter)
			{
				_fieldList.Add(m.Member);
			}

			return m;
		}
	}

	public class Item
	{
		public string Name;
		public int Value;
	}
}
