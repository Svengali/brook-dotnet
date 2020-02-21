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
using System.IO;
using Piot.Brook.Shared;

namespace Piot.Brook.Octet
{
    public class OutOctetStream : IOutOctetStream
    {
        BinaryWriter writer;
        MemoryStream memoryStream;

        public OutOctetStream()
        {
            memoryStream = new MemoryStream();
            writer = new BinaryWriter(memoryStream);
        }

        public void WriteUint16(ushort data)
        {
            Write(EndianConverter.Uint16ToBytes(data));
        }

        public void WriteUint32(uint data)
        {
            Write(EndianConverter.Uint32ToBytes(data));
        }

        public void WriteUint64(ulong data)
        {
            Write(EndianConverter.Uint64ToBytes(data));
        }

        public void WriteUint8(byte data)
        {
            writer.Write(data);
        }

        public void Write(byte[] data)
        {
            writer.Write(data);
        }

        public void WriteOctet(byte v)
        {
            WriteUint8(v);
        }

        public void WriteOctets(byte[] data)
        {
            writer.Write(data);
        }

        public int RemainingOctetCount { get; }

        public byte[] Close()
        {
            var writeBuf = memoryStream.GetBuffer();
            var octetsWritten = (int)memoryStream.Length;
            var bufferToReturn = new byte[octetsWritten];

            Buffer.BlockCopy(writeBuf, 0, bufferToReturn, 0, octetsWritten);

            return bufferToReturn;
        }
    }
}
