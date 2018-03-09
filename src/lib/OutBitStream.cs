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
		IOctetWriter octetWriter;
		int remainingBits = 32;
		uint ac;

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

		public void Flush()
		{
			WriteOctets();
		}

		uint MaskFromCount(int count)
		{
			return ((uint)1 << count) - 1;
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

		void WriteRest(uint v, int count, int bitsToKeepFromLeft)
		{
			var ov = v;

			ov >>= (count - bitsToKeepFromLeft);
			ov &= MaskFromCount(bitsToKeepFromLeft);
			ov <<= remainingBits - bitsToKeepFromLeft;
			remainingBits -= bitsToKeepFromLeft;
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
			remainingBits = 32;
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
			if (count > 32)
			{
				throw new Exception("Max 32 bits to write ");
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
	}
}
