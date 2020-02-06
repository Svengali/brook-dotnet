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

    public class InBitStreamTest
    {
        static IInBitStream Setup(byte[] octets)
        {
            var octetReader = new OctetReader(octets);
            var bitStream = new InBitStream(null, octetReader, octets.Length * 8);

            return bitStream;
        }

        [Fact]
        public static void ReadNibble()
        {
            var bitStream = Setup(new byte[] { 0x3c });

            var t = bitStream.ReadBits(2);

            Assert.Equal((uint)0, t);

            var t2 = bitStream.ReadBits(1);
            Assert.Equal((uint)1, t2);

            var t3 = bitStream.ReadBits(1);
            Assert.Equal((uint)1, t3);
        }

        [Fact]
        public static void ReadTooFar()
        {
            var bitStream = Setup(new byte[] { 0xfe });

            var t = bitStream.ReadBits(4);

            Assert.Equal((uint)15, t);

            Assert.Throws<EndOfStreamException>(() => bitStream.ReadBits(5));
        }

        [Fact]
        public static void ReadOverDWord()
        {
            var bitStream = Setup(new byte[] { 0xca, 0xfe, 0xba, 0xdb, 0xee, 0xf0 });

            var t = bitStream.ReadBits(24);

            Assert.Equal((uint)(0xcafeba), t);

            var t2 = bitStream.ReadBits(16);

            Assert.Equal((uint)(0xdbee), t2);
        }
    }
}
