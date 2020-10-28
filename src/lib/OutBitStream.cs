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
using System;

namespace Piot.Brook
{
    public class OutBitStream : IOutBitStream
    {
        const int AccumulatorSize = 32;
        const uint MaxFilter = 0xffffffff;

        IOctetWriter octetWriter;
        int remainingBits = AccumulatorSize;
        uint ac;
        uint position;

        public OutBitStream(IOctetWriter octetWriter)
        {
            this.octetWriter = octetWriter;
        }

        public void WriteUint16(ushort v)
        {
            WriteBits(v, 16);
        }

        public void WriteInt16(short v)
        {
            WriteSignedBits(v, 16);
        }

        public void WriteUint32(uint v)
        {
            WriteBits(v, 32);
        }

        public void WriteUint64(ulong v)
        {
            WriteBits((uint)(v >> 32), 32);
            WriteBits((uint)(v & 0xffffffff), 32);
        }

        public void WriteUint8(byte v)
        {
            WriteBits(v, 8);
        }

        public void WriteFromStream(IInBitStream inBitStream, int bitCount)
        {
            const int ChunkBitSize = AccumulatorSize;
            var restBitCount = bitCount % ChunkBitSize;
            var chunkCount = bitCount / ChunkBitSize;

            for (var i = 0; i < chunkCount; ++i)
            {
                var data = inBitStream.ReadRawBits(ChunkBitSize);
                WriteRawBits(data, ChunkBitSize);
            }

            if (restBitCount > 0)
            {
                var restData = inBitStream.ReadRawBits(restBitCount);
                WriteRawBits(restData, restBitCount);
            }
        }

        public void Flush()
        {
            WriteLast();
        }

        public uint RemainingBitCount
        {
            get
            {
                return (uint)(octetWriter.Length * 8) - position;
            }
        }

        uint MaskFromCount(int count)
        {
            if (count < AccumulatorSize)
            {
                return ((uint)1 << count) - 1;
            }
            else
            {
                return MaxFilter;
            }
        }

        void WriteOctet(byte v)
        {
            octetWriter.WriteOctet(v);
        }

        public uint Accumulator
        {
            get
            {
                return ac;
            }
        }

        public uint Tell
        {
            get
            {
                return position;
            }
        }

        public void Rewind(uint newPosition)
        {
            if (newPosition > position)
            {
                throw new ArgumentOutOfRangeException("Can't rewind forwards.");
            }
            else if (newPosition < position)
            {
                WriteLast();

                var leftOverBits = newPosition % 8;
                var octets = newPosition / 8;
                octets = Math.Min(octets, (uint)octetWriter.Octets.Length - 1);
                if (leftOverBits > 0)
                {
                    remainingBits = AccumulatorSize - (int)leftOverBits;
                    uint bits = octetWriter.Octets[octets];
                    var mask = ~MaskFromCount(remainingBits);
                    ac = (bits << AccumulatorSize - 8) & mask;
                }
                octetWriter.Rewind(octets);
                position = newPosition;
            }
        }

        void WriteRest(uint v, int count, int bitsToKeepFromLeft)
        {
            var ov = v;

            ov >>= (count - bitsToKeepFromLeft);
            ov &= MaskFromCount(bitsToKeepFromLeft);
            ov <<= remainingBits - bitsToKeepFromLeft;
            remainingBits -= bitsToKeepFromLeft;
            position += (uint)bitsToKeepFromLeft;
            ac |= ov;
        }

        public static byte[] GetBytes(uint value)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value));
        }

        static byte[] ReverseAsNeeded(byte[] bytes)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return bytes;
            }
            else
            {
                Array.Reverse(bytes);
                return bytes;
            }
        }

        void WriteOctets()
        {
            var octets = GetBytes(ac);

            octetWriter.WriteOctets(octets);
            ac = 0;
            remainingBits = AccumulatorSize;
        }

        void WriteLast()
        {
            if (remainingBits == AccumulatorSize)
            {
                return;
            }

            var bitsWritten = AccumulatorSize - remainingBits;
            var octetCount = ((bitsWritten - 1) / 8) + 1;
            for (var i = 0; i < octetCount && octetWriter.RemainingOctetCount > 0; i++)
            {
                var outOctet = (byte)((ac & 0xff000000) >> 24);
                ac <<= 8;
                octetWriter.WriteOctet(outOctet);
            }

            ac = 0;
            remainingBits = AccumulatorSize;
        }

        public void WriteSignedBits(int v, int count)
        {
            var sign = v < 0 ? 1 : 0;

            if (sign != 0)
            {
                v = -v;
            }
            WriteBits((uint)sign, 1);
            WriteBits((uint)v, count - 1);
        }

        public void WriteBits(uint v, int count)
        {
            if (count > AccumulatorSize)
            {
                throw new Exception($"Max {AccumulatorSize} bits to write ");
            }

            if (count > RemainingBitCount)
            {
                throw new Exception($"Attempting to write too many bits. {count} > {RemainingBitCount} ");
            }

            if (count > remainingBits)
            {
                var firstWriteCount = remainingBits;
                WriteRest(v, count, firstWriteCount);
                WriteOctets();
                WriteRest(v, count - firstWriteCount, count - firstWriteCount);
            }
            else
            {
                WriteRest(v, count, count);
            }
        }

        public void WriteRawBits(uint v, int count)
        {
            WriteBits(v, count);
        }
    }
}
