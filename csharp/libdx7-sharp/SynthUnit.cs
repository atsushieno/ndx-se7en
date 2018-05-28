using System;
using System.Runtime.CompilerServices;
namespace NDXSe7en
{
	public class SynthUnit : IDisposable
	{
		IntPtr handle;

		public SynthUnit (RingBuffer ringBuffer)
		{
			handle = Natives.synth_unit_new (ringBuffer.Handle);
		}

		internal IntPtr Handle => handle;

		public void Dispose ()
		{
			Natives.synth_unit_delete (handle);
		}

		public void Init (double sampleRate)
		{
			Natives.synth_unit_init (handle, sampleRate);
		}

		public void GetSamples (short [] buffer, int start, int length)
		{
			unsafe {
				fixed (short* ptr = buffer) {
					GetSamples (length, (IntPtr) (ptr + start));
				}
			}
		}

		public void GetSamples (int nSamples, IntPtr shortBuffer)
		{
			Natives.synth_unit_get_samples (handle, nSamples, shortBuffer);
		}
	}
}
