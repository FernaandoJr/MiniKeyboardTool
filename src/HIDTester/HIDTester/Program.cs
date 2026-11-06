using System;
using System.Threading;
using System.Windows.Forms;

namespace HIDTester
{
	internal static class Program
	{
		/// <summary>
		/// Lets the Inno Setup installer (AppMutex=MiniKeyboardToolAppSingleInstance)
		/// detect and politely close a running instance, and gives users a clear
		/// message instead of two instances fighting over the HID handle.
		/// </summary>
		private static Mutex singleInstanceMutex;

		[STAThread]
		private static void Main()
		{
			bool createdNew;
			singleInstanceMutex = new Mutex(true, "MiniKeyboardToolAppSingleInstance", out createdNew);
			if (!createdNew)
			{
				MessageBox.Show("MiniKeyboardTool is already running.", "MiniKeyboardTool",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FormMain());
		}
	}
}
