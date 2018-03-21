/*

MIT License

Copyright (c) 2017 Peter Bjorklund

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/
using Piot.Brook;

namespace Tests
{
	using Xunit;

	public class InOutBitStreamTest
	{
		static OutBitStream Setup(out OctetWriter octetWriter)
		{
			octetWriter = new OctetWriter(128);
			var bitStream = new OutBitStream(octetWriter);

			return bitStream;
		}

		static IInBitStream SetupIn(OctetWriter writer)
		{
			var octetReader = new OctetReader(writer.Octets);
			var bitStream = new InBitStream(octetReader, writer.Octets.Length * 8);

			return bitStream;
		}

		[Fact]
		public static void WriteAndReadSigned16bit()
		{
			OctetWriter writer;
			var outStream = Setup(out writer);
			var v = (short)-23988;

			outStream.WriteInt16(v);
			outStream.Flush();
			var inStream = SetupIn(writer);
			var rv = inStream.ReadInt16();
			Assert.Equal(-23988, rv);
		}

		[Fact]
		public static void WriteTwoNumbers()
		{
			OctetWriter writer;
			var outStream = Setup(out writer);
			const short otherValue = 1234;
			var v = (short)-23988;
			var u = otherValue;

			outStream.WriteBits(2, 3);
			outStream.WriteInt16(v);
			outStream.WriteInt16(u);
			outStream.Flush();
			var inStream = SetupIn(writer);
			var x = inStream.ReadBits(3);
			Assert.Equal((uint)2, x);
			var rv = inStream.ReadInt16();
			Assert.Equal(-23988, rv);
			var ru = inStream.ReadInt16();
			Assert.Equal(otherValue, ru);
		}
	}
}
