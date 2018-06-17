using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terminal.Gui;
using NStack;

namespace NDXSe7en.Demo.TerminalClient
{
	class Driver
	{
		public static void Main (string [] args)
		{
			new Driver ().Run ();
		}

		Model model;
		Controller controller;
		MenuBar menu;

		public Driver ()
		{
			model = new Model ();
			controller = new Controller (model);
		}

		public void Run ()
		{
			Application.Init ();

			var top = Application.Top;
			var win = new Window (new Rect (0, 0, top.Frame.Width, top.Frame.Height), "NDXSe7en");
			top.Add (win);

			menu = BuildMenuBar ();
			top.Add (menu);

			var text = new TextField ("1   v120 o4 l2 cdefgab>c");
			win.Add (text);

			var button = new Button ("Play");
			button.Clicked = () => controller.PlayMml (text.Text.ToString ());
			win.Add (button);

			model.PatchDirectoryUpdated += OnUpdatePatchDirectory;

			controller.StartMain ();

			Application.Run ();
		}

		void PerformQuitApplicationCommand ()
		{
			Application.Top.Running = false;
		}

		void PerformConfigurePatchDirectoryCommand ()
		{
			var d = new OpenDialog ("Configure patch directory", "Choose a directory that contains DX7 patch files") {
				CanChooseDirectories = true,
				CanChooseFiles = false
			};
			Application.Run (d);
			if (!ustring.IsNullOrEmpty (d.FilePath))
				controller.SetPatchDirectory (Path.Combine (d.DirectoryPath.ToString (), d.FilePath.ToString ()));
		}

		MenuBar BuildMenuBar ()
		{
			menu = new MenuBar (new MenuBarItem [] {
				new MenuBarItem ("_File", new MenuItem [] {
					new MenuItem ("_Configure patch directory", "configures patch directory", PerformConfigurePatchDirectoryCommand),
					new MenuItem ("E_xit", "quits application", PerformQuitApplicationCommand)
				}),
				BuildPatchMenu ()
			});
			return menu;
		}

		MenuBarItem BuildPatchMenu ()
		{
			var ret = new MenuBarItem ("_Patches", new MenuItem [] {
			});
			return ret;
		}

		void OnUpdatePatchDirectory ()
		{
			menu.Menus [Array.IndexOf (menu.Menus, menu.Menus.First (i => i.Title == "_Patches"))] = BuildPatchMenu ();
		}
	}
}
