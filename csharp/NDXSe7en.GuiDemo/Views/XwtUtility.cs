using System;
using Xwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace NDXSe7en.GuiDemo.Views
{
	public class XwtUtility
	{
		public static Menu BuildDirectoryTree (DirectoryInfo directory, Action<string> action, Func<FileSystemInfo,bool> filter = null)
		{
			Console.WriteLine (directory.FullName);
			var ret = new Menu ();
			foreach (var subdir in directory.GetDirectories ().Where (d => filter == null || filter (d))) {
				var dirmenu = new MenuItem (subdir.Name);
				dirmenu.Clicked += delegate { dirmenu.SubMenu = BuildDirectoryTree (subdir, action, filter); };
				ret.Items.Add (dirmenu);
			}
			foreach (var file in directory.GetFiles ().Where (f => filter == null || filter (f))) {
				var filemenu = new MenuItem (file.Name);
				filemenu.Clicked += delegate { action (file.FullName); };
				ret.Items.Add (filemenu);
			}
			return ret;
		}
	}
}
