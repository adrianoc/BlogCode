using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;

namespace WWWTC_XII
{
	class Program
	{
		[DllImport("ole32.dll")]
		static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

		static void Main()
		{
			IRunningObjectTable rot;
			if (GetRunningObjectTable(0, out rot) == 0)
			{

			}

			string databaseFileName = "wwwtc-xii.odb";
			DeleteDatabase(databaseFileName);
			using (var db = Db4oEmbedded.OpenFile(databaseFileName))
			{
				User fooManager = new User("Foo", null)
										{
											Addresses = new List<Address>
											{
												new Address("Street1"), 
												new Address("Street2")
											}
										};

				User fooBarManager = new User("FooBar", fooManager);
				User barManager = new User("John Doe",  fooBarManager);
				User user = new User("Adriano",  barManager);
				
				db.Store(user);
			}

			using (var db = Db4oEmbedded.OpenFile(databaseFileName))
			{
				var query = db.Query();
				query.Constrain(typeof (User));
				query.Descend("Name").Constrain("Adriano");

				User user = (User) query.Execute()[0];
				Console.WriteLine(user.Manager.Manager.Manager.Addresses[0].Street);
			}
		}

		class User
		{
			public string Name;
			public User Manager;
			public IList<Address> Addresses;

			public User(string name, User manager)
			{
				Name = name;
				Manager = manager;
			}
		}

		class Address
		{
			public string Street;

			public Address(string street)
			{
				Street = street;
			}
		}

		private static void DeleteDatabase(string fileName)
		{
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
		}
	}
}
