using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o.Linq.Internals;
using Db4oLinqExtensions=DYK_II___LINQ_Projections.ProjectionSupport.Linq.Db4oLinqExtensions;

namespace DYK_II___LINQ_Projections.ProjectionSupport
{
	internal class ProjectionEnabledRunner
	{
		public ProjectionEnabledRunner(IObjectContainer container)
		{
			IDb4oLinqQuery<string> result = Db4oLinqExtensions.Select(container.Cast<Item>(), item => item.Name);
			foreach (string name in result)
			{
			}
			//foreach (var name in (from Item item in container select item.Name))
			//{
			//}
		}

		public void Run()
		{
			throw new NotImplementedException();
		}
	}
}

namespace DYK_II___LINQ_Projections.ProjectionSupport.Linq
{
	internal static class Db4oLinqExtensions
	{
		public static IDb4oLinqQuery<TTarget> Select<TSource, TTarget>(this IDb4oLinqQuery<TSource> db4oLinqQuery, Expression<Func<TSource, TTarget>> selector)
		{
			Db4oQuery<TSource> query = (Db4oQuery<TSource>) db4oLinqQuery;
			IExtObjectSet result = query.Execute().Ext();

			return new Projection<TSource, TTarget>((IInternalObjectContainer) query.QueryFactory,  result, selector);
		}
	}

	internal class Projection<TSource, TTarget> : IDb4oLinqQuery<TTarget>
	{
		private IExtObjectSet _objectSet;
		private IInternalObjectContainer _container;

		public Projection(IInternalObjectContainer container, IExtObjectSet set, Expression<Func<TSource, TTarget>> selectorExpression)
		{
			_container = container;
			_objectSet = set;
		}

		public IEnumerator<TTarget> GetEnumerator()
		{
			foreach (var id in _objectSet.GetIDs())
			{
				yield return default(TTarget);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
