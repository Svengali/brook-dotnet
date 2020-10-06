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
    public class OctetWriter : IOctetWriter
    {
        readonly byte[] data;
        int position;

        public OctetWriter(int size)
        {
            data = new byte[size];
        }

        public void WriteOctet(byte v)
        {
            data[position++] = v;
        }

        public byte[] Octets
        {
            get
            {
                var clone = new byte[position];
                Array.Copy(data, 0, clone, 0, position);
                return clone;
            }
        }

        public void WriteOctets(byte[] v)
        {
            if (v.Length > RemainingOctetCount)
            {
                throw new Exception($"written too far. Wanted to write {v.Length} octets, but only {RemainingOctetCount} remains. (buffer size: {data.Length})");
            }
            var octetCount = v.Length;

            Array.Copy(v, 0, data, position, octetCount);
            position += octetCount;
        }


        public uint Tell
        {
            get
            {
                return (uint)position;
            }
        }

        public uint Length
        {
            get
            {
                return (uint)data.Length;
            }
        }

        public uint RemainingOctetCount
        {
            get
            {
                return Length - Tell;
            }
        }
    }
}
