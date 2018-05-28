using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SoundIOSharp;
using System.Runtime.CompilerServices;
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
			ring_buffer = new RingBuffer ();
			synth = new SynthUnit (ring_buffer);
			synth.Init (44100);

			soundio = new SoundIO ();
			soundio.Connect ();
			soundio.FlushEvents ();
			Console.WriteLine ("SoundIO backend: " + soundio.CurrentBackend);

			// MIDI sender
			var midi_wait = new ManualResetEvent (false);
			Task.Run (() => {
				midi_wait.WaitOne ();
				if (args.Length > 0) {
					var sysex = File.ReadAllBytes (args [0]);
					int remaining = sysex.Length;
					while (remaining > 0) {
						int len = Math.Min (remaining, ring_buffer.WriteBytesAvailable ());
						ring_buffer.Write (sysex, 0, len);
						Task.Delay (50);
						remaining -= len;
					}
				}
				for (int i = 0; i < 10; i++) {
					byte [] bytes = { 0x90, (byte) (0x30 + i), 0x60 };
					ring_buffer.Write (bytes, 0, 3);
					System.Threading.Thread.Sleep (1000);
					byte [] bytes2 = { 0x80, (byte) (0x30 + i), 0 };
					ring_buffer.Write (bytes2, 0, 3);
				}
			});

			// Audio outputter
			var wait = new ManualResetEvent (false);
			Task.Run (() => {
				var device = soundio.GetOutputDevice (soundio.DefaultOutputDeviceIndex);
				if (device.ProbeError != 0) {
					Console.Error.WriteLine ($"Cannot probe device {device.Name}.");
					return;
				}
				Console.WriteLine ($"Output device: {device.Name}");
				var outStream = device.CreateOutStream ();
				if (!device.SupportsFormat (SoundIOFormat.S16LE))
					throw new NotSupportedException ();
				outStream.Format = SoundIOFormat.S16LE;
				outStream.WriteCallback = (min, max) => WriteCallback (outStream, min, max);
				outStream.UnderflowCallback = () => { Console.WriteLine ("underflow"); };
				outStream.ErrorCallback = () => {
					Console.WriteLine ($"ERROR at libsoundio: {outStream.LayoutErrorMessage}");
				};
				outStream.Open ();
				play_audio = true;
				midi_wait.Set ();
				Task.Delay (50);
				outStream.Start ();
				soundio.FlushEvents ();

				wait.WaitOne ();
				outStream.Dispose ();
				device.RemoveReference ();
			});

			Console.Read ();
			play_audio = false;
			wait.Set ();
			Console.WriteLine ("Finishing up");
			soundio.Dispose ();
		}

		RingBuffer ring_buffer;
		SynthUnit synth;
		SoundIO soundio;
		bool play_audio;
		short [] samples = new short [8192 * 3];

		void WriteCallback (SoundIOOutStream outStream, int min, int max)
		{
			if (!play_audio)
				return;

			Console.WriteLine ($"WriteCallback invoked: {min} / {max}");

			double float_sample_rate = outStream.SampleRate;
			double seconds_per_frame = 1.0 / float_sample_rate;

			//if (max - min > samples.Length)
			//	samples = new short [max - min];

#if test_with_sine
			//double seconds_offset = 0;
#else
			synth.GetSamples (samples, 0, samples.Length);
#endif
			//if (samples.Any (s => s != 0))
			//    Console.WriteLine (string.Concat (samples.Take (50).Select (s => s.ToString ("X04"))));
			int frameRemaining = max;
			int frameCount = frameRemaining;
			while (play_audio && frameRemaining > 0) {
				var results = outStream.BeginWrite (ref frameCount);
				if (frameCount == 0)
					break;

#if test_with_sine
				double pitch = 440.0;
				double radians_per_second = pitch * 2.0 * Math.PI;
				for (int frame = 0; frame < frameCount; frame += 1) {
					double sample = Math.Sin ((seconds_offset + frame * seconds_per_frame) * radians_per_second);
					for (int channel = 0; channel < outStream.Layout.ChannelCount; channel += 1) {

						var area = results.GetArea (channel);
						double range = (double)short.MaxValue - (double)short.MinValue;
						double val = sample * range / 2.0;
						unsafe { *((short*)area.Pointer) = (short)val; }
						area.Pointer += area.Step;
					}
				}
				seconds_offset = Math.IEEERemainder (seconds_offset + seconds_per_frame * frameCount, 1.0);
#else
				for (int i = 0; i < outStream.Layout.ChannelCount; i++) {
					var area = results.GetArea (i);
					unsafe {
						Marshal.Copy (samples, 0, area.Pointer, samples.Length);
						area.Pointer += area.Step * samples.Length;
					}
				}
#endif

				outStream.EndWrite ();

				frameRemaining -= frameCount;
			}
		}
	}
}
