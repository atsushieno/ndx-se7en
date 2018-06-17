using System;
using System.IO;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using Commons.Music.Midi.Mml;
using Commons.Music.Midi;

namespace NDXSe7en
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

		public async Task PlayMml (string mml)
		{
			await Task.Run (() => {
				try {
					var compiler = new MmlCompiler ();
Console.WriteLine ("Compiling...");
					var music = compiler.Compile (false, mml);
Console.WriteLine ("Compiled"); // we're debugging how compilation process gets stuck in the middle...
					var player = new MidiPlayer (music);
					player.EventReceived += (m) => {
						if (m.Data != null)
							synthesizer.SendMidi (m.Data, 0, m.Data.Length);
						else
							synthesizer.SendMidi (m.StatusByte, m.Msb, m.Lsb);
					};
						player.PlayAsync ();
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}
			});
		}

		public async Task LoadCachedState ()
		{
			await Task.Run (() => {
				using (var store = IsolatedStorageFile.GetUserStoreForAssembly ()) {
					if (!store.FileExists ("settings.txt"))
						return;
					using (var file = store.OpenFile ("settings.txt", FileMode.Open)) {
						string dir = new StreamReader (file).ReadToEnd ();
						if (Directory.Exists (dir))
							SetPatchDirectoryInternal (dir, true);
					}
				}
			});
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

		async Task SetPatchDirectoryInternal (string directory, bool skipSave)
		{
			PatchDirectory = directory;
			if (!skipSave)
				await SaveCachedState ();
			await Task.Run (PatchDirectoryUpdated);
		}

		public async Task SetPatchDirectory (string directory)
		{
			await SetPatchDirectoryInternal (directory, false);
		}

		public event Action PatchDirectoryUpdated;
	}
}
