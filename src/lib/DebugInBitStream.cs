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
    public class DebugInBitStream : IInBitStream
    {
        readonly IInBitStream bitStream;
        ILog log;

        public DebugInBitStream(ILog log, IInBitStream bitStream)
        {
            this.log = log;
            this.bitStream = bitStream;
        }

        void CheckType(DebugSerializeType expectedType, int expectedBitCount)
        {
            var type = (DebugSerializeType)bitStream.ReadBits(4);
            var bitCount = bitStream.ReadBits(7);

            if (bitCount != expectedBitCount)
            {
                throw new Exception($"Expected type {expectedType} bitcount {expectedBitCount} received type {type} {bitCount}");
            }

            if (type != expectedType)
            {
                throw new Exception($"Expected type {expectedType} received {type}");
            }

        }

        public ushort ReadUint16()
        {
            CheckType(DebugSerializeType.Uint16, 16);
            return bitStream.ReadUint16();
        }

        public int ReadSignedBits(int count)
        {
            CheckType(DebugSerializeType.SignedBits, count);
            return bitStream.ReadSignedBits(count);
        }

        public bool IsEof => bitStream.IsEof;

        public short ReadInt16()
        {
            CheckType(DebugSerializeType.Int16, 16);
            return bitStream.ReadInt16();
        }

        public uint ReadUint32()
        {
            CheckType(DebugSerializeType.Uint32, 32);
            return bitStream.ReadUint32();
        }

        public ulong ReadUint64()
        {
            CheckType(DebugSerializeType.Uint64, 64);
            return bitStream.ReadUint64();
        }

        public byte ReadUint8()
        {
            CheckType(DebugSerializeType.Uint8, 8);
            return bitStream.ReadUint8();
        }

        public uint ReadBits(int count)
        {
            CheckType(DebugSerializeType.UnsignedBits, count);
            return bitStream.ReadBits(count);
        }

        public uint ReadRawBits(int count)
        {
            return bitStream.ReadBits(count);
        }

        uint InternalReadBits(int count)
        {
            return bitStream.ReadBits(count);
        }
    }
}
