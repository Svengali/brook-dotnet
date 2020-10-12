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
            var bitStream = new InBitStream(null, octetReader, writer.Octets.Length * 8);

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

        [Fact]
        public static void WriteSomeNumbersThenRewind()
        {
            OctetWriter writer;
            var outStream = Setup(out writer);

            outStream.WriteBits(2, 3);
            outStream.WriteBits(7, 4);
            var position = outStream.Tell;
            outStream.WriteInt16(-69);
            outStream.Rewind(position);
            Assert.Equal(position, outStream.Tell);
            outStream.Flush();

            Assert.Equal(position, outStream.Tell);
            var inStream = SetupIn(writer);
            var x = inStream.ReadBits(3);
            Assert.Equal((uint)2, x);
            var y = inStream.ReadBits(4);
            Assert.Equal((uint)7, y);
        }

        [Fact]
        public static void WriteSomeNumbersThenRewindThenMoreNumbers()
        {
            OctetWriter writer;
            var outStream = Setup(out writer);

            outStream.WriteBits(2, 3);
            outStream.WriteBits(7, 4);
            var position = outStream.Tell;
            outStream.WriteInt16(-69); //Throw away
            outStream.Rewind(position);
            Assert.Equal(position, outStream.Tell);
            outStream.WriteInt16(45);
            outStream.WriteUint32(69696969);
            outStream.Flush();

            var inStream = SetupIn(writer);
            var x = inStream.ReadBits(3);
            Assert.Equal((uint)2, x);
            var y = inStream.ReadBits(4);
            Assert.Equal((uint)7, y);
            var z = inStream.ReadInt16();
            Assert.Equal(45, z);
            var zz = inStream.ReadUint32();
            Assert.Equal((uint)69696969, zz);
        }

        [Fact]
        public static void WriteSomeNumbersThenRewindPartWay()
        {
            OctetWriter writer;
            var outStream = Setup(out writer);

            outStream.WriteBits(2, 3);
            outStream.WriteBits(7, 3);
            outStream.WriteInt16(-69);
            outStream.WriteInt16(45);
            outStream.WriteUint32(69696969);
            var position = outStream.Tell;
            outStream.WriteInt16(-17);
            outStream.WriteUint32(123345);
            outStream.Rewind(position);
            outStream.Flush();

            var inStream = SetupIn(writer);
            var a = inStream.ReadBits(3);
            Assert.Equal((uint)2, a);
            var b = inStream.ReadBits(3);
            Assert.Equal((uint)7, b);
            var c = inStream.ReadInt16();
            Assert.Equal(-69, c);
            var d = inStream.ReadInt16();
            Assert.Equal(45, d);
            var e = inStream.ReadUint32();
            Assert.Equal((uint)69696969, e);
        }

        [Fact]
        public static void WriteRandomNumbersRewindThenKnownNumbers()
        {
            OctetWriter writer;
            var outStream = Setup(out writer);
            var rand = new System.Random();

            for (int i = 0; i < 50; i++)
            {
                var kind = rand.Next();
                switch (kind % 5)
                {
                    case 0: outStream.WriteBits(5, 5); break;
                    case 1: outStream.WriteInt16(-69); break;
                    case 2: outStream.WriteUint16(1234); break;
                    case 3: outStream.WriteUint8(69); break;
                    default: break;
                }
            }

            outStream.Rewind(0);

            outStream.WriteBits(2, 3);
            outStream.WriteBits(7, 3);
            outStream.WriteInt16(-69);
            outStream.Flush();

            var inStream = SetupIn(writer);
            var a = inStream.ReadBits(3);
            Assert.Equal((uint)2, a);
            var b = inStream.ReadBits(3);
            Assert.Equal((uint)7, b);
            var c = inStream.ReadInt16();
            Assert.Equal(-69, c);
        }

        [Fact]
        public static void WriteNumbersThenRandomNumbersRewindThenKnownNumbers()
        {
            OctetWriter writer;
            var outStream = Setup(out writer);
            var rand = new System.Random();

            outStream.WriteBits(2, 3);
            outStream.WriteBits(7, 3);
            outStream.WriteInt16(-69);

            var position = outStream.Tell;

            for (int i = 0; i < 50; i++)
            {
                var kind = rand.Next();
                switch (kind % 5)
                {
                    case 0: outStream.WriteBits(5, 5); break;
                    case 1: outStream.WriteInt16(-69); break;
                    case 2: outStream.WriteUint16(1234); break;
                    case 3: outStream.WriteUint8(69); break;
                    default: break;
                }
            }

            outStream.Rewind(position);

            outStream.WriteBits(4, 3);
            outStream.WriteBits(5, 3);
            outStream.WriteInt16(45);
            outStream.Flush();

            var inStream = SetupIn(writer);
            var a = inStream.ReadBits(3);
            Assert.Equal((uint)2, a);
            var b = inStream.ReadBits(3);
            Assert.Equal((uint)7, b);
            var c = inStream.ReadInt16();
            Assert.Equal(-69, c);

            var d = inStream.ReadBits(3);
            Assert.Equal((uint)4, d);
            var e = inStream.ReadBits(3);
            Assert.Equal((uint)5, e);
            var f = inStream.ReadInt16();
            Assert.Equal(45, f);
        }
    }
}
