using System;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace DR_RTM
{

	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			Application.Run(new Form1());
			}
		}
	}
