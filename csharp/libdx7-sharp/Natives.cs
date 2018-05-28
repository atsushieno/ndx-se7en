using System;
using System.Runtime.InteropServices;

namespace NDXSe7en
{
	class Natives
	{
		public const string LibraryName = "dxse7en";

		[DllImport (LibraryName)]
		public static extern IntPtr ring_buffer_new ();
		[DllImport (LibraryName)]
		public static extern void ring_buffer_delete (IntPtr instance);
		[DllImport (LibraryName)]
		public static extern int ring_buffer_bytes_available (IntPtr instance);
		[DllImport (LibraryName)]
		public static extern int ring_buffer_write_bytes_available (IntPtr instance);
		[DllImport (LibraryName)]
		public static extern int ring_buffer_read (IntPtr instance, int size, IntPtr/* uint8_t* */ bytes);
		[DllImport (LibraryName)]
		public static extern void ring_buffer_write (IntPtr instance, IntPtr bytes, int size);

		[DllImport (LibraryName)]
		public static extern IntPtr synth_unit_new (IntPtr ring_buffer);
		[DllImport (LibraryName)]
		public static extern void synth_unit_delete (IntPtr instance);
		[DllImport (LibraryName)]
		public static extern void synth_unit_init (IntPtr instance, double sample_rate);
		[DllImport (LibraryName)]
		public static extern void synth_unit_get_samples (IntPtr instance, int n_samples, IntPtr/* int16_t* */ buffer);
	}
}
