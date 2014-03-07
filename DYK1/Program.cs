using System;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;

namespace DYK1
{
	class Program
	{
		static void Main(string[] args)
		{
			const string databaseFileName = "dyk1.odb";
			TryDeleteFile(databaseFileName);
			using (var db = Db4oEmbedded.OpenFile(databaseFileName))
			{
				db.Store(new Person("Adriano", new Address("You know where I live")));
				db.Store(new Person("Sulivan", new Address("Monstropolis")));
				db.Store(new Person("Myke", new Address("Who cares?")));
				db.Store(new Address("Foo address"));
				
				IQuery q =  db.Query();
				q.Constrain(typeof(Person));
				IQuery descendantQuery = q.Descend("_address");
				
				foreach(var result in descendantQuery.Execute())
				{
					Console.WriteLine(result);
				}
			}
		}

		private static void TryDeleteFile(string fileName)
		{
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
		}
	}

	class Address
	{
		public Address(string streetName)
		{
			StreetName = streetName;
		}

		public string StreetName { get; set; }

		public override string ToString()
		{
			return "Address: " + StreetName;
		}
	}

	class Person
	{
		public Person(string name, Address address)
		{
			Name = name;
			_address = address;
		}

		public Address Address
		{
			get { return _address; }
			set { _address = value; }
		}

		public string Name { get; set;}

		public override string ToString()
		{
			return "Person: " + Name;
		}
		
		private Address _address;
	}
}
