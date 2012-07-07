using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace GenericConstraintsInCSharp
{
	public class Driver
	{
		public static void Main(string[] args)
		{
			Console.Error.WriteLine();
			var assembly = AssemblyDefinition.ReadAssembly(args[0]);

			var type = assembly.MainModule.Types.SingleOrDefault(candidate => candidate.Name == args[1]);
			if (type == null)
			{
				Console.Error.WriteLine("Could not find type: " + args[1]);
				if (args.Length > 2 && args[2] == "-t")
				{
					foreach (var t in assembly.MainModule.Types)
					{
						Console.WriteLine("\t{0}", t.Name);
					}
				}

				Environment.Exit(-1);
			}

			if (!type.HasGenericParameters)
			{
				Console.Error.WriteLine("Type {0} has no generic parameters.", type);
				Environment.Exit(-1);
			}

			Console.WriteLine();
			foreach (var genericParameter in type.GenericParameters)
			{
				Console.WriteLine("\t{0} : {1}", genericParameter.Name, CollectConstraints(genericParameter));
			}

			if (args.Length == 3)
			{
				var tbu = args[2].Split(':');
				var genericParameter = type.GenericParameters.SingleOrDefault(param => param.Name == tbu[0]);
				if (genericParameter == null)
				{
					Console.Error.WriteLine("Generic parameter '{0}' not found in type '{1}'.", tbu[0], type.FullName);
					Environment.Exit(-2);
				}

				Type constraintType = null;
				switch (tbu[1][0])
				{
					case 'a':
					case 'A':
						constraintType = typeof (Array);
						break;

					case 'e':
					case 'E':
						constraintType = typeof (Enum);
						break;

					case 'd':
					case 'D':
						constraintType = typeof (Delegate);
						break;
				}

				genericParameter.Constraints.Add(assembly.MainModule.Import(constraintType));

				Console.WriteLine(assembly.MainModule);
				assembly.Write("c:\\temp\\" + assembly.MainModule);
			}
		}

		private static string CollectConstraints(GenericParameter param)
		{
			List<string> constraints = new List<string>();

			if (param.HasDefaultConstructorConstraint)
			{
				constraints.Add("new()");
			}

			if(param.HasReferenceTypeConstraint)
			{
				constraints.Add("class");
			}

			constraints.AddRange(param.Constraints.Select(constraint => constraint.ToString()));

			return string.Join(", ", constraints.ToArray());
		}
	}
}
