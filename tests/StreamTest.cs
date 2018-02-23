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

namespace NUnit.Tests
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	public class InBitStreamTest
	{
		static IInBitStream Setup(byte[] octets)
		{
			var octetReader = new OctetReader(octets);
			var bitStream = new InBitStream(octetReader);

			return bitStream;
		}

		[Test]
		public static void ReadNibble()
		{
			var bitStream = Setup(new byte[] {0x3c});

			var t = bitStream.ReadBits(2);

			Assert.That(t, Is.EqualTo(0));

			var t2 = bitStream.ReadBits(1);
			Assert.That(t2, Is.EqualTo(1));

			var t3 = bitStream.ReadBits(1);
			Assert.That(t3, Is.EqualTo(1));
		}

		[Test]
		public static void ReadTooFar()
		{
			var bitStream = Setup(new byte[] {0xfe});

			var t = bitStream.ReadBits(4);

			Assert.That(t, Is.EqualTo(15));

			Assert.Throws<EndOfStreamException>(() => bitStream.ReadBits(5));
		}

		[Test]
		public static void ReadOverDWord()
		{
			var bitStream = Setup(new byte[] {0xca, 0xfe, 0xba, 0xdb, 0xee, 0xf0});

			var t = bitStream.ReadBits(24);

			Assert.That(t, Is.EqualTo(0xcafeba));

			var t2 = bitStream.ReadBits(16);

			Assert.That(t2, Is.EqualTo(0xdbee));
		}
	}
}
