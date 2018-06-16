using System;
using System.IO;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace NDXSe7en.GuiDemo
{
	public class Model
	{
		DX7Synthesizer synthesizer;

		public Model ()
		{
			synthesizer = new DX7Synthesizer ();
		}

		public DX7Synthesizer Synthesizer => synthesizer;

		public async Task LoadPatch (string patchFile)
		{
			await Task.Run (() => {
				var bytes = File.ReadAllBytes (patchFile);
				Synthesizer.SendMidi (bytes, 0, bytes.Length);
			});
		}

		public async Task PlayTestSound ()
		{
			await Task.Run (() => {
				for (int i = 0; i < 10; i++) {
					byte [] bytes = { 0x90, (byte)(0x30 + i), 0x60 };
					Synthesizer.SendMidi (bytes, 0, 3);
					System.Threading.Thread.Sleep (1000);
					byte [] bytes2 = { 0x80, (byte)(0x30 + i), 0 };
					Synthesizer.SendMidi (bytes2, 0, 3);
				}
			});
		}

		public async Task LoadCachedState ()
		{
			using (var store = IsolatedStorageFile.GetUserStoreForAssembly ()) {
				if (!store.FileExists ("settings.txt"))
					return;
				using (var file = store.OpenFile ("settings.txt", FileMode.Open)) {
					string dir = new StreamReader (file).ReadToEnd ();
					if (Directory.Exists (dir))
						await SetPatchDirectory (dir, true);
				}
			}
		}

		async Task SaveCachedState ()
		{
			await Task.Run (() => {
				using (var store = IsolatedStorageFile.GetUserStoreForAssembly ()) {
					using (var file = store.CreateFile ("settings.txt")) {
						using (var sw = new StreamWriter (file))
							sw.Write (PatchDirectory);
					}
				}
			});
		}

		public string PatchDirectory { get; private set; }

		async Task SetPatchDirectory (string directory, bool skipSave)
		{
			PatchDirectory = directory;
			if (!skipSave)
				await SaveCachedState ();
			await Task.Run (PatchDirectoryUpdated);
		}

		public async Task SetPatchDirectory (string directory)
		{
			await SetPatchDirectory (directory, false);
		}

		public event Action PatchDirectoryUpdated;
	}
}
