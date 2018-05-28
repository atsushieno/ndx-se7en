using System;
using System.IO;

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

		public void LoadPatch (string patchFile)
		{
			var bytes = File.ReadAllBytes (patchFile);
			Synthesizer.SendMidi (bytes, 0, bytes.Length);
		}

		public void PlayTestSound ()
		{
			for (int i = 0; i < 10; i++) {
				byte [] bytes = { 0x90, (byte)(0x30 + i), 0x60 };
				Synthesizer.SendMidi (bytes, 0, 3);
				System.Threading.Thread.Sleep (1000);
				byte [] bytes2 = { 0x80, (byte)(0x30 + i), 0 };
				Synthesizer.SendMidi (bytes2, 0, 3);
			}
		}
	}
}
