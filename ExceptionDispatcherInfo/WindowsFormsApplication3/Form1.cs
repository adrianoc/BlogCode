using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			button2.Click += delegate
			{
				//Foo2();
				MessageBox.Show(this[ n:0 ]);
				MessageBox.Show(this[ n:1 ]);
			};
		}

		private void button1_Click(object sender, EventArgs e)
		{
			ThrowInThreadPool();

			//try
			//{
			//	DoItAsync();
			//}
			//catch (Exception ex)
			//{
			//	MessageBox.Show("IN MAIN " + ex);
			//}
		}

		private static async void DoItAsync()
		{
			try
			{
				await Task.Run(() =>
				{
					throw new Exception("WTF ?");
				});
			}
			catch (Exception ex)
			{
				MessageBox.Show("??? " + ex);
			}
		}


		private void ThrowInThreadPool()
		{
			var exceptions = new BlockingCollection<ExceptionDispatchInfo>();
			ThreadPool.QueueUserWorkItem(_ =>
			{
				try
				{
					Foo();
				}
				catch (Exception ex)
				{
					ExceptionDispatchInfo edi = ExceptionDispatchInfo.Capture(ex);
					exceptions.Add(edi);
				}
				exceptions.CompleteAdding();
			});

			foreach (ExceptionDispatchInfo edi in exceptions.GetConsumingEnumerable())
			{
				try
				{
					edi.Throw();
				}
				catch (Exception ex)
				{
					listBox1.Items.Add(ex);
				}
			}
		}

		private string this[int n, [CallerMemberName] string caller = null]
		{
			get
			{
				return n > 0 ? this[n-1] : caller + "(" + n + ")";
			}
		}

		private void Foo2([CallerMemberName] string caller = null)
		{
			MessageBox.Show("Called from : " + caller);
		}

		private void Foo([CallerMemberName] string caller = null)
		{
			throw new Exception("Called from: " + caller);
		}
	}

}
