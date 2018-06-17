using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Linq;

namespace NDXSe7en.Demo
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			new MainClass ().Run (args);
		}

		void Run (string [] args)
		{

#if false
			var synthesizer = new DX7Synthesizer ();

			// MIDI sender
			var midi_wait = new ManualResetEvent (false);
			Task.Run (() => {
				midi_wait.WaitOne ();
				if (args.Length > 0) {
					var sysex = File.ReadAllBytes (args [0]);
					synthesizer.SendMidi (sysex, 0, sysex.Length);
				}
				for (int i = 0; i < 10; i++) {
					synthesizer.SendMidi (0x90, (byte)(0x30 + i), 0x60);
					Thread.Sleep (1000);
					synthesizer.SendMidi (0x90, (byte)(0x30 + i), 0);
				}
			});

			// Audio outputter
			var wait = new ManualResetEvent (false);
			synthesizer.Start ();

			midi_wait.Set ();
			Console.Read ();
			Console.WriteLine ("Finishing up");
			synthesizer.Stop ();
#else
			var model = new Model ();
			var controller = new Controller (model);
			controller.StartMain ();
			controller.PlayMml ("1   l2 o4 v120 cdefgab>c");
			Console.Read ();
			controller.StopMain ().Wait ();
#endif
		}
	}
}
