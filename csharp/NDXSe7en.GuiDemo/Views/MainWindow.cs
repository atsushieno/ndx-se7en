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
			Application.Initialize ();
			new MainWindow ().Show ();
			Application.Run ();
		}

		Model model;
		Controller controller;

		public MainWindow ()
		{
			model = new Model ();
			controller = new Controller (model);

			this.Width = 600;
			this.Height = 400;
			this.Closed += delegate {
				model.Synthesizer.Stop ();
				Application.Exit ();
			};

			this.MainMenu = new Menu ();

			var file = new MenuItem ("_File");
			file.SubMenu = new Menu ();
			this.MainMenu.Items.Add (file);

			var configPatchDir = new MenuItem ("_Configure Patch directory");
			configPatchDir.Clicked += delegate {
				var dirdlg = new SelectFolderDialog ("Choose Patch directory.") {
					Multiselect = false,
				};
				if (dirdlg.Run (this))
					controller.SetPatchDirectory (dirdlg.Folder);
			};
			file.SubMenu.Items.Add (configPatchDir);

			var exit = new MenuItem ("E_xit");
			exit.Clicked += (o, e) => Close ();
			file.SubMenu.Items.Add (exit);

			var patch = new MenuItem ("_Patch");
			this.MainMenu.Items.Add (patch);

			model.PatchDirectoryUpdated += delegate { patch.SubMenu = XwtUtility.BuildDirectoryTree (new DirectoryInfo (model.PatchDirectory), s => Task.Run (() => controller.SelectPatchFile (s))); };

			var button = new Button ("PLAY");
			button.Clicked += delegate {
				Task.Run (() => controller.PlayTestSound ());
			};
			this.Content = button;

			controller.StartMain ();
		}
	}
}
