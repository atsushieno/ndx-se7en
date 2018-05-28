using System;
using Xunit;
using System.Threading.Tasks;
using System.Linq;

namespace NDXSe7en.Tests
{
	public class SynthUnitTest
	{
		[Fact]
		public void BasicOperations ()
		{
			using (var buf = new RingBuffer ()) {
				Assert.Equal (0, buf.BytesAvailable ());
				Assert.Equal (8191, buf.WriteBytesAvailable ()); // implementation dependent.
				using (var synth = new SynthUnit (buf)) {
					var bytes = new byte [] { 0x90, 0x30, 0 }; // note on at 0x30, but with no velocity = no sound.
					buf.Write (bytes, 0, bytes.Length);
					var results = new short [1024];
					Assert.Equal (3, buf.BytesAvailable ());
					synth.Init (44100);
					synth.GetSamples (results, 0, results.Length);
					Assert.All (results, r => Assert.Equal (0, r));

					bytes = new byte [] { 0x90, 0x30, 100 }; // now give vel = 100
					buf.Write (bytes, 0, bytes.Length);
					synth.GetSamples (results, 0, results.Length);
					Assert.Contains (results, r => r != 0);

					// to see what's being generated, enable these lines.
					//var str = string.Concat (results.Select (s => s.ToString ("X04")));
					//Console.WriteLine (str);
				}
			}
		}
	}
}
