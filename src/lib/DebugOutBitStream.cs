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
using Piot.Log;

namespace Piot.Brook
{
    public class DebugOutBitStream : IOutBitStream
    {
        readonly IOutBitStream bitStream;
        readonly ILog log;

        public DebugOutBitStream(ILog log, IOutBitStream bitStream)
        {
            this.bitStream = bitStream;
            this.log = log;
        }

        public void WriteUint16(ushort v)
        {
            WriteType(DebugSerializeType.Uint16, 16);
            bitStream.WriteUint16(v);
        }

        public void WriteInt16(short v)
        {
            WriteType(DebugSerializeType.Int16, 16);
            bitStream.WriteInt16(v);
        }

        public void WriteUint32(uint v)
        {
            WriteType(DebugSerializeType.Uint32, 32);
            bitStream.WriteUint32(v);
        }

        public void WriteUint64(ulong v)
        {
            WriteType(DebugSerializeType.Uint64, 64);
            bitStream.WriteUint64(v);
        }

        public void WriteUint8(byte v)
        {
            WriteType(DebugSerializeType.Uint8, 8);
            bitStream.WriteUint8(v);
        }

        public void WriteFromStream(IInBitStream inBitStream, int bitCount)
        {
            bitStream.WriteFromStream(inBitStream, bitCount);
        }

        public uint Tell
        {
            get
            {
                return bitStream.Tell;
            }
        }

        public void WriteSignedBits(int v, int count)
        {
            WriteType(DebugSerializeType.SignedBits, count);
            bitStream.WriteSignedBits(v, count);
        }

        public void WriteBits(uint v, int count)
        {
            WriteType(DebugSerializeType.UnsignedBits, count);
            bitStream.WriteBits(v, count);
        }

        void WriteType(DebugSerializeType type, int bitCount)
        {
            // log.Debug($"Type {type} bitcount {bitCount}");
            InternalWriteBits((uint)type, 4);
            InternalWriteBits((uint)bitCount, 7);
        }

        void InternalWriteBits(uint v, int count)
        {
            bitStream.WriteBits(v, count);
        }

        public void WriteRawBits(uint v, int count)
        {
            InternalWriteBits(v, count);
        }

        public void Flush()
        {
            bitStream.Flush();
        }
    }
}
