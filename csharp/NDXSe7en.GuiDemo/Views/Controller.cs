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

		public void SelectPatchFile (string patchFile)
		{
			Model.LoadPatch (patchFile);
		}

		public void PlayTestSound ()
		{
			Model.PlayTestSound ();
		}
	}
}
