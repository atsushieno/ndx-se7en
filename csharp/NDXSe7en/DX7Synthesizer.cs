using System;
using SoundIOSharp;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NDXSe7en
{
	public class DX7Synthesizer
	{
		RingBuffer ring_buffer;
		SynthUnit synth;
		SoundIO soundio;

		public DX7Synthesizer ()
		{
			ring_buffer = new RingBuffer ();
			synth = new SynthUnit (ring_buffer);
			synth.Init (44100);
		}

		public void SendMidi (byte [] bytes, int offset, int length)
		{
			int off = offset;
			int remaining = length;
			while (remaining > 0) {
				int len = Math.Min (remaining, ring_buffer.WriteBytesAvailable ());
				ring_buffer.Write (bytes, off, len);
				off += len;
				remaining -= len;
			}
		}

		readonly byte [] msg_store = new byte [3];
		public void SendMidi (byte statusCode, byte msb, byte lsb)
		{
			var type = statusCode & 0xF0;
			int size = (type == 0xC0 || type == 0xA0) ? 2 : 3;
			msg_store [0] = statusCode;
			msg_store [1] = msb;
			msg_store [2] = lsb;
			ring_buffer.Write (msg_store, 0, size);
		}

		public enum SynthesizerState
		{
			Initial,
			Started,
			Finished,
		}

		public SynthesizerState State { get; private set; }

		SoundIOOutStream out_stream;

		public void Start ()
		{
			if (State != SynthesizerState.Initial)
				return;

			soundio = new SoundIO ();
			soundio.Connect ();
			soundio.FlushEvents ();
			var device = soundio.GetOutputDevice (soundio.DefaultOutputDeviceIndex);
			if (device.ProbeError != 0)
				throw new DX7SynthesizerException ($"Cannot probe device {device.Name}.");
			out_stream = device.CreateOutStream ();
			if (!device.SupportsFormat (SoundIOFormat.S16LE))
				throw new NotSupportedException ();
			out_stream.Format = SoundIOFormat.S16LE;
			out_stream.SampleRate = 44100;
			out_stream.WriteCallback = (min, max) => WriteCallback (min, max);
			out_stream.UnderflowCallback = () => { Console.WriteLine ("underflow"); };
			out_stream.ErrorCallback = () => {
				throw new DX7SynthesizerException ($"ERROR at libsoundio: {out_stream.LayoutErrorMessage}");
			};
			out_stream.Open ();
			State = SynthesizerState.Started;
			out_stream.Start ();
			soundio.FlushEvents ();
		}

		public void Stop ()
		{
			if (State != SynthesizerState.Started)
				return;

			State = SynthesizerState.Finished;
			out_stream.Dispose ();
			soundio.Disconnect ();
			soundio.Dispose ();
		}

		short [] samples = new short [8192 * 10];

		void WriteCallback (int min, int max)
		{
			if (State != SynthesizerState.Started)
				return;

			if (max - min > samples.Length)
				samples = new short [max - min];

			int frameRemaining = max - min;
Console.Write ($"WriteCallback:");
			while (State == SynthesizerState.Started && frameRemaining > 0) {
				int frameCount = frameRemaining;
				var results = out_stream.BeginWrite (ref frameCount);
				if (frameCount == 0)
					break;
Console.WriteLine ($"                  {min}, {max} -> {frameCount} / { frameRemaining}");

				synth.GetSamples (samples, 0, frameCount);

				for (int i = 0; i < results.ChannelCount; i++) {
					var area = results.GetArea (i);
					int len = Math.Min (max - min, results.FrameCount);
					Marshal.Copy (samples, min, area.Pointer, frameCount);
					area.Pointer += area.Step * frameCount;
				}

				out_stream.EndWrite ();

				frameRemaining -= frameCount;
			}
		}

		[System.Serializable]
		public class DX7SynthesizerException : Exception
		{
			public DX7SynthesizerException ()
			{
			}

			public DX7SynthesizerException (string message) : base (message)
			{
			}

			public DX7SynthesizerException (string message, Exception inner) : base (message, inner)
			{
			}

			protected DX7SynthesizerException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
			{
			}
		}
	}
}
