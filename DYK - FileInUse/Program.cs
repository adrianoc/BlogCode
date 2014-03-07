using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace DYK_II___FileInUse
{
	class Program
	{
		[DllImport("ole32.dll")]
		static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);
		
		[DllImport("ole32.dll")]
		static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

		static void Main()
		{
			IRunningObjectTable rot;
			if (GetRunningObjectTable(0, out rot) == 0)
			{
				IEnumMoniker enumerator;
				rot.EnumRunning(out enumerator);

				if (enumerator != null)
				{
					IMoniker[] monikers = new IMoniker[1];
					IntPtr countPtr = Marshal.AllocCoTaskMem(4);
					while(enumerator.Next(1, monikers, countPtr) == 0)
					{
						DisplayMonikerInfo(monikers);
						Marshal.ReleaseComObject(monikers[0]);
					}

					Marshal.ReleaseComObject(enumerator);
					Marshal.FreeCoTaskMem(countPtr);
				}
			}

			Marshal.ReleaseComObject(rot);
		}

		private static void DisplayMonikerInfo(IMoniker[] monikers)
		{
			IBindCtx bindCtx;
			CreateBindCtx(0, out bindCtx);
			string displayName;
			monikers[0].GetDisplayName(bindCtx, null, out displayName);

			Console.WriteLine("[Moniker]  {0}", displayName);

			Marshal.ReleaseComObject(bindCtx);
		}
	}
}
