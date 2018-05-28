using System;

namespace NDXSe7en
{
	public class RingBuffer : IDisposable
	{
		IntPtr handle;

		public RingBuffer ()
		{
			handle = Natives.ring_buffer_new ();
		}

		internal IntPtr Handle => handle;

		public void Dispose ()
		{
			Natives.ring_buffer_delete (handle);
		}

		public int BytesAvailable ()
		{
			return Natives.ring_buffer_bytes_available (handle);
		}

		public int WriteBytesAvailable ()
		{
			return Natives.ring_buffer_write_bytes_available (handle);
		}

		public int Read (byte [] bytes, int offset, int length)
		{
			unsafe {
				fixed (byte* ptr = bytes) {
					return Read (length, (IntPtr) (ptr + offset));
				}
			}
		}

		public int Read (int size, IntPtr bytes)
		{
			return Natives.ring_buffer_read (handle, size, bytes);
		}

		public void Write (byte [] bytes, int offset, int length)
		{
			unsafe {
				fixed (byte* ptr = bytes) {
					Write ((IntPtr) (ptr + offset), length);
				}
			}
		}

		public void Write (IntPtr bytes, int size)
		{
			Natives.ring_buffer_write (handle, bytes, size);
		}
	}
}
