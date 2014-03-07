using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.TA;
using DYK_II___LINQ_Projections.ProjectionSupport;

namespace DYK_II___LINQ_Projections
{
	class Program : TempFileBasedDb4oSample
	{
		private const int ItemsCount = 100;

		static void Main(string[] args)
		{
			new Program().Run();
		}

		private void Run()
		{
			DeleteDatabaseFile();

			Store(Items());

			//MeasureTime("LINQ default Activation depth", IterateOverNameProperty());
			
			//MeasureTime("LINQ Activation Depth = 1, no TA",
			//            config =>
			//            {
			//                config.Common.ObjectClass(typeof (Item)).MaximumActivationDepth(1);
			//                config.Common.ObjectClass(typeof (Item)).MinimumActivationDepth(1);
			//            },

			//            IterateOverNameProperty());

			//MeasureTime(
			//        "LINQ with TA Enabled",
			//        config => config.Common.Add(new TransparentActivationSupport()),
			//        IterateOverNameProperty());

			MeasureTime("LINQ with Projection Support (No TA, Default Activation Depth)",
						delegate
						{
							new ProjectionEnabledRunner(Db()).Run();
						});

			MeasureTime("Native Query default activation depth",
						delegate
						{
							IQuery query = Db().Query();
							query.Constrain(typeof(Item));

							foreach (Item item in query.Execute())
							{
							}
						});

			MeasureTime(
					"NQ -> Direct field access (no index)",
					config => config.Common.ObjectClass(typeof(Item)).ObjectField("_name").Indexed(false),
					NQDirectFieldAccessRunner());

			MeasureTime(
					"NQ -> Direct field access (indexed)",
					config => config.Common.ObjectClass(typeof(Item)).ObjectField("_name").Indexed(true),
					NQDirectFieldAccessRunner());
		}

		private Action IterateOverNameProperty()
		{
			return delegate
			       	{
			       		foreach (var name in (from Item item in Db() select item.Name))
			       		{
					
			       		}
			       	};
		}

		private Action NQDirectFieldAccessRunner()
		{
			return delegate
			       	{
			       		IQuery query = Db().Query();
			       		query.Constrain(typeof (Item));
			       		IObjectSet result = query.Execute();
			       		string[] path = new[] { "_name" };

			       		foreach (long id in result.Ext().GetIDs())
			       		{
			       			object obj = Db().Ext().GetByID(id);
			       			Db().Ext().Descend(obj, path);
			       		}
			       	};
		}

		private void MeasureTime(string message, Action action)
		{
			MeasureTime(message, delegate { }, action);
		}

		private void MeasureTime(string message, Action<IEmbeddedConfiguration> configSetup, Action action)
		{
			long time = MeasureTime(configSetup, action);
			Console.WriteLine("{0} : {1}", message, time);
		}

		private IEnumerable<Item> Items()
		{
			for (int i = 0; i < ItemsCount; i++)
			{
				yield return new Item { Name = "Item #" + i , Parent = new Item {Name = "Item #" + i + " parent."}};
			}
		}
	}

	internal class TempFileBasedDb4oSample
	{
		protected void DeleteDatabaseFile()
		{
			if (File.Exists(_databasePath))
			{
				File.Delete(_databasePath);
			}
		}

		protected long MeasureTime(Action<IEmbeddedConfiguration> configSetup, Action action)
		{
			GC.Collect();
			IEmbeddedConfiguration config = NewConfiguration(configSetup);

			Stopwatch stopwatch = new Stopwatch();
			
			WithContainer(config, delegate
			                      	{
										stopwatch.Start();
			                      		action();
										stopwatch.Stop();
			                      	});
			
			return stopwatch.ElapsedMilliseconds;
		}

		private void WithContainer(IEmbeddedConfiguration config, Action action)
		{
			using (_container = Db4oEmbedded.OpenFile(config, _databasePath))
			{
				action();
			}
			_container = null;
		}

		private IEmbeddedConfiguration NewConfiguration(Action<IEmbeddedConfiguration> configSetup)
		{
			IEmbeddedConfiguration config = Db4oEmbedded.NewConfiguration();
			configSetup(config);

			return config;
		}

		protected void Store<T>(IEnumerable<T> objs)
		{
			WithContainer(
				Db4oEmbedded.NewConfiguration(),
				delegate
				{
					foreach (T obj in objs)
					{
						Db().Store(obj);
					}
				});
		}

		protected IObjectContainer Db()
		{
			if (_container == null)
			{
				throw new InvalidOperationException();
			}

			return _container;
		}

		//private string _databasePath = Path.GetTempFileName();
		private string _databasePath = @"C:\Users\adriano\AppData\Local\Temp\tmp1F6E.tmp";
		private IEmbeddedObjectContainer _container;
	}

	class Item : IActivatable
	{
		private Item _parent;
		public Item Parent
		{
			get
			{
				Activate(ActivationPurpose.Read);
				return _parent;
			}

			set
			{
				Activate(ActivationPurpose.Write);
				_parent = value ;
			}
		}
		public byte[] Data = new byte[10000];

		private string _name;
		private IActivator _activator;

		public string Name 
		{ 
			get 
			{
				Activate(ActivationPurpose.Read);
				return _name;
			}  

			set
			{
				Activate(ActivationPurpose.Write);
				_name = value;
			} 
		}

		public void Bind(IActivator activator)
		{
			if (_activator != null && activator != null && activator != _activator)
			{
				throw new InvalidOperationException();	
			}

			_activator = activator;
		}

		public void Activate(ActivationPurpose purpose)
		{
			if (_activator != null)
			{
				_activator.Activate(purpose);
			}
		}
	}
}
