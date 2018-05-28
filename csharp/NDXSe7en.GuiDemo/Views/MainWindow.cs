using System;
using Xwt;
using NDXSe7en.GuiDemo.Views;
using System.IO;
using System.Threading.Tasks;

namespace NDXSe7en.GuiDemo
{
	class MainWindow : Window
	{
		public static void Main (string [] args)
		{
			Application.Initialize (ToolkitType.Gtk);
			new MainWindow ().Show ();
			Application.Run ();
		}

		Model model;
		Controller controller;

		public MainWindow ()
		{
			model = new Model ();
			controller = new Controller (model);
			model.Synthesizer.Start ();

			this.Width = 600;
			this.Height = 400;
			this.Closed += delegate {
				model.Synthesizer.Stop ();
				Application.Exit ();
			};

			this.MainMenu = new Menu ();
			var file = new MenuItem ("_File");
			file.SubMenu = XwtUtility.BuildDirectoryTree (new DirectoryInfo ("/home/atsushi/Desktop/Dexed_cart_1.0"), s => Task.Run (() => controller.SelectPatchFile (s)));
			this.MainMenu.Items.Add (file);


			var button = new Button ("PLAY");
			button.Clicked += delegate {
				Task.Run (() => controller.PlayTestSound ());
			};
			this.Content = button;
		}
	}
}
