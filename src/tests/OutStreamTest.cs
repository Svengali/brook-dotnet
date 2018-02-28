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
﻿
using Piot.Brook;

namespace Tests
{
	using System;
	using Xunit;

	public class OutStreamTest
	{
		static OutBitStream Setup(out OctetWriter octetWriter)
		{
			octetWriter = new OctetWriter(128);
			var bitStream = new OutBitStream(octetWriter);

			return bitStream;
		}

		[Fact]
		public static void WriteNibble()
		{
			OctetWriter writer;
			var bitStream = Setup(out writer);

			bitStream.WriteBits(0, 2);
			Assert.Equal((byte)0, bitStream.Accumulator);
			bitStream.WriteBits(1, 1);
			Assert.Equal((uint)0x20000000, bitStream.Accumulator);
			bitStream.WriteBits(1, 1);
			Assert.Equal((uint)0x30000000, bitStream.Accumulator);
		}

		[Fact]
		public static void WriteMoreThanOctet()
		{
			OctetWriter writer;
			var bitStream = Setup(out writer);

			bitStream.WriteBits(0xf, 4);
			Assert.Equal(0xf0000000, bitStream.Accumulator);
			bitStream.WriteBits(0x3, 2);
			Assert.Equal(0xfc000000, bitStream.Accumulator);
			bitStream.WriteBits(0x5, 4);
			Assert.Equal(0xfd400000, bitStream.Accumulator);
		}

		static uint ConvertFromNetworkOrderUint32(byte[] octets)
		{
			var four = new byte[4];

			Array.Copy(octets, four, 4);

			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(four);
			}

			return BitConverter.ToUInt32(four, 0);
		}

		[Fact]
		public static void WriteMoreThan32bit()
		{
			OctetWriter writer;
			var bitStream = Setup(out writer);

			bitStream.WriteBits(0xfefe, 16);
			Assert.Equal(0xfefe0000, bitStream.Accumulator);
			bitStream.WriteBits(0xcd31, 15);
			Assert.Equal(0xfefe9a62, bitStream.Accumulator);
			bitStream.WriteBits(0x3, 2);
			var octets = writer.Octets;
			var stored = ConvertFromNetworkOrderUint32(octets);
			Assert.Equal(0xfefe9a63, stored);
			Assert.Equal(0x80000000, bitStream.Accumulator);
		}
	}
}
