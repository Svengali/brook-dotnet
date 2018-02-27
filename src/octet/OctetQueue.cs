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
﻿using System;

namespace Piot.Brook.Shared
{
	public sealed class OctetQueue
	{
		private int _head;
		private int _tail;
		private int _size;
		private byte[] _buffer;

		public int Length
		{
			get
			{
				return _size;
			}
		}

		public OctetQueue(int capacity)
		{
			_buffer = new byte[capacity];
		}

		void Clear()
		{
			_head = 0;
			_tail = 0;
			_size = 0;
		}

		public void Enqueue(byte[] buffer, int offset, int size)
		{
			if (size == 0)
			{
				return;
			}

			if ((_size + size) > _buffer.Length)
			{
				throw new Exception("Buffer is out of capacity");
			}

			if (_head < _tail)
			{
				var rightLength = (_buffer.Length - _tail);

				if (rightLength >= size)
				{
					Buffer.BlockCopy(buffer, offset, _buffer, _tail, size);
				}
				else
				{
					Buffer.BlockCopy(buffer, offset, _buffer, _tail, rightLength);
					Buffer.BlockCopy(buffer, offset + rightLength, _buffer, 0, size - rightLength);
				}
			}
			else
			{
				Buffer.BlockCopy(buffer, offset, _buffer, _tail, size);
			}

			_tail = (_tail + size) % _buffer.Length;
			_size += size;
		}

		public int Peek(byte[] buffer, int offset, int size)
		{
			if (size > _size)
			{
				size = _size;
			}

			if (size == 0)
			{
				return 0;
			}

			if (_head < _tail)
			{
				Buffer.BlockCopy(_buffer, _head, buffer, offset, size);
			}
			else
			{
				var rightLength = (_buffer.Length - _head);

				if (rightLength >= size)
				{
					Buffer.BlockCopy(_buffer, _head, buffer, offset, size);
				}
				else
				{
					Buffer.BlockCopy(_buffer, _head, buffer, offset, rightLength);
					Buffer.BlockCopy(_buffer, 0, buffer, offset + rightLength, size - rightLength);
				}
			}

			return size;
		}

		public void Skip(int size)
		{
			if (size > _size)
			{
				throw new Exception(string.Format("Can not skip %d", size));
			}

			_head = (_head + size) % _buffer.Length;
			_size -= size;

			if (_size == 0)
			{
				_head = 0;
				_tail = 0;
			}
		}

		public override string ToString()
		{
			return string.Format("[OctetQueue: length={0} (head:{1}, tail:{2})]", Length, _head, _tail);
		}
	}
}
