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
using System;

namespace Piot.Brook
{
	public class InBitStream : IInBitStream
	{
		IOctetReader octetReader;
		int remainingBits;
		uint data;

		public InBitStream(IOctetReader octetReader)
		{
			this.octetReader = octetReader;
		}

		public ushort ReadUint16()
		{
			return (ushort) ReadBits(16);
		}

		public int ReadSignedBits(int count)
		{
			var sign = ReadBits(1);
			var v = (int)ReadBits(count - 1);

			if (sign != 0)
			{
				v = -v;
			}

			return v;
		}

		public short ReadInt16()
		{
			return (short)ReadSignedBits(16);
		}

		public uint ReadUint32()
		{
			return ReadBits(32);
		}

		public ulong ReadUint64()
		{
			var upper = (ulong)ReadBits(32);
			var result = upper << 32;
			var lower = (ulong)ReadBits(32);

			result |= lower;

			return result;
		}

		public byte ReadUint8()
		{
			return (byte) ReadBits(8);
		}

		uint MaskFromCount(int count)
		{
			return ((uint)1 << count) - 1;
		}

		uint ReadOnce(int bitsToRead)
		{
			if (bitsToRead == 0)
			{
				return 0;
			}

			if (bitsToRead > remainingBits)
			{
				throw new EndOfStreamException();
			}
			var mask = MaskFromCount(bitsToRead);
			var shiftPos = (remainingBits - bitsToRead);
			var bits = (data >> shiftPos) & mask;
			// Console.Error.WriteLine("READ mask {0:X} shift:{1} bits:{2:X} data:{3:X} {4:X}", mask, shiftPos, bits, data, (data >> shiftPos));
			remainingBits -= bitsToRead;
			return bits;
		}

		void Fill()
		{
			var octetsToRead = 4;

			if (octetsToRead > octetReader.RemainingOctetCount)
			{
				octetsToRead = octetReader.RemainingOctetCount;
			}

			var newData = (uint)0;
			for (var i = 0; i < octetsToRead; ++i)
			{
				newData <<= 8;
				var octet = octetReader.ReadOctet();
				newData |= (uint)octet;
			}

			data = newData;
			remainingBits = octetsToRead * 8;
			// Console.Error.WriteLine("Data is now {0:X} octetsToRead:{1} Remaining:{2}", data, octetsToRead, remainingBits);
		}

		public uint ReadBits(int count)
		{
			if (count > 32)
			{
				throw new Exception("Max 32 bits to read");
			}

			if (count > remainingBits)
			{
				var secondCount = count - remainingBits;
				var v = ReadOnce(remainingBits);
				Fill();
				v <<= secondCount;
				v |= ReadOnce(secondCount);
				return v;
			}
			else
			{
				return ReadOnce(count);
			}
		}
	}
}
