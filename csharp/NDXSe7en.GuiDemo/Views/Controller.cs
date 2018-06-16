using System;
using System.IO;
using System.Threading.Tasks;
namespace NDXSe7en.GuiDemo.Views
{
	public class Controller
	{
		public Controller (Model model)
		{
			Model = model;
		}

		public Model Model { get; private set; }

		public async Task SelectPatchFile (string patchFile)
		{
			await Model.LoadPatch (patchFile);
		}

		public async Task PlayMml (string mml)
		{
			await Model.PlayMml (mml);
		}

		public async Task SetPatchDirectory (string directory)
		{
			await Model.SetPatchDirectory (directory);
		}

		public async Task StartMain ()
		{
			await Model.LoadCachedState ();
			Model.Synthesizer.Start ();
		}
	}
}
