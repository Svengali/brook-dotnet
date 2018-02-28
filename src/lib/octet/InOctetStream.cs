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
﻿ using System;
using Piot.Brook.Shared;

namespace Piot.Brook.Octet
{
	public class InOctetStream : IInOctetStream
	{
		readonly byte[] octets;
		int pos;

		public int RemainingOctetCount
		{
			get
			{
				return octets.Length - pos;
			}
		}

		public InOctetStream(byte[] data)
		{
			// this.log = log.CreateLog(typeof(OctetReader));
			octets = data;
			pos = 0;
			// log.Debug("OctetReader:{0}", data.Length);
		}

		public ushort ReadUint16()
		{
			return EndianConverter.BytesToUint16(ReadOctets(2));
		}

		public uint ReadUint32()
		{
			return EndianConverter.BytesToUint32(ReadOctets(4));
		}

		public ulong ReadUint64()
		{
			return EndianConverter.BytesToUint64(ReadOctets(8));
		}

		public byte ReadUint8()
		{
			var v = octets[pos++];

			return v;
		}

		public byte[] ReadOctets(int octetCount)
		{
			if (pos + octetCount > octets.Length)
			{
				var e = new Exception(string.Format("Reading too far {0} {1}", pos, octetCount));
				//log.Exception(e);
				throw e;
			}
			var buf = new byte[octetCount];

			Buffer.BlockCopy(octets, pos, buf, 0, octetCount);
			pos += octetCount;

			return buf;
		}
	}
}
