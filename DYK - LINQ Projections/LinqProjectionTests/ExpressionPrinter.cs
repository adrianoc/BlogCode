using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;
using Db4objects.Db4o.Linq.Expressions;

namespace LinqProjectionTests
{
	internal class ExpressionPrinter : ExpressionTransformer
	{
		private StringBuilder _asString = new StringBuilder();

		public ExpressionPrinter(Expression expression)
		{
			Visit(expression);
		}

		protected override Expression VisitLambda(LambdaExpression lambda)
		{
			VisitParameterList(lambda.Parameters);
			_asString.Append("=>");
			Visit(lambda.Body);

			return lambda;
		}

		private void VisitParameterList(ReadOnlyCollection<ParameterExpression> parameters)
		{
			foreach (var parameter in parameters)
			{
				Visit(parameter);
			}
		}

		protected override Expression VisitParameter(ParameterExpression p)
		{
			_asString.AppendFormat("P[{0}:{1}]", p.Name, p.Type.Name);
			return p;
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			_asString.Append(c.Value);
			return c;
		}

		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			Visit(m.Expression);
			_asString.AppendFormat(".{0}", m.Member.Name);

			return m;
		}

		protected override Expression VisitBinary(BinaryExpression b)
		{
			_asString.Append("<");
			Visit(b.Left);
			_asString.AppendFormat("[{0}]", b.NodeType);
			Visit(b.Right);
			_asString.Append(">");

			return b;
		}

		protected override NewExpression VisitNew(NewExpression nex)
		{
			_asString.Append("new{");
			VisitExpressions(nex.Arguments);
			_asString.Append("}");
			return nex;
		}

		private void VisitExpressions(ReadOnlyCollection<Expression> expressions)
		{
			foreach (var expression in expressions)
			{
				Visit(expression);
			}
		}

		public override string ToString()
		{
			return _asString.ToString();
		}
	}
}
