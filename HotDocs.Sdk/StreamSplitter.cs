/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System.IO;

namespace HotDocs.Sdk.Cloud
{
	/// <summary>
	/// This class copies an input stream to an output stream or to multiple output streams.
	/// You can copy a certain number of bytes, or copy until a given pattern is found.
	/// It uses a circular buffer for streaming.
	/// </summary>
	internal class StreamSplitter
	{
		#region Private constants
		private const int _defaultBufferSize = 16 * 1024;
		#endregion

		#region Private fields
		private readonly byte[] _buffer;
		private int _head;
		private int _virtualSize;
		private readonly int _bufferSize;
		private Stream _streamIn;
		#endregion

		#region Constructors
		internal StreamSplitter()
		{
			_buffer = new byte[_defaultBufferSize];
			_bufferSize = _defaultBufferSize;
		}

		internal StreamSplitter(int bufferSize)
		{
			_buffer = new byte[bufferSize];
			_bufferSize = bufferSize;
		}
		#endregion

		#region Internal methods
		/// <summary>
		/// Start a new parsing session.
		/// </summary>
		/// <param name="streamIn"></param>
		internal void Init(Stream streamIn)
		{
			_streamIn = streamIn;
			_head = 0;
			_virtualSize = 0;
		}

		/// <summary>
		/// Copies N bytes to the output stream.
		/// </summary>
		/// <param name="streamOut"></param>
		/// <param name="count"></param>
		internal void WriteBytes(Stream streamOut, int count)
		{
			while (count > 0)
			{
				FillFromStream();
				count -= WriteToStream(streamOut, Min(count, _virtualSize));
			}
		}

		/// <summary>
		/// Copies from the input to the output stream until it finds the pattern.
		/// The pattern is not written to the output, and is skipped in the input.
		/// </summary>
		/// <param name="streamOut"></param>
		/// <param name="pattern"></param>
		internal void WriteUntilPattern(Stream streamOut, byte[] pattern)
		{
			int indexOfPattern;

			do
			{
				int bytesRead = FillFromStream();
				indexOfPattern = IndexOf(pattern);
				if (indexOfPattern == -1)
				{
					if (bytesRead == 0) // End of stream, and no more patterns in buffer
					{
						throw new EndOfStreamException();
					}
					// Here we try to be clever by leaving pattern.Length-1 bytes unwritten.
					// That way, if we have half of the pattern at the end of the buffer, it will be joined with the
					// other half when we fill the buffer again, making a whole pattern,
					// which will be detected in the next call to IndexOf(pattern).
					WriteToStream(streamOut, _virtualSize - (pattern.Length - 1));
				}
			}
			while (indexOfPattern == -1);

			// Write out the rest until the pattern, then skip the pattern.
			WriteToStream(streamOut, indexOfPattern);
			Skip(pattern.Length);
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This reads as many bytes as it can to fill the empty space in the buffer.
		/// </summary>
		/// <returns>The number of bytes read.</returns>
		private int FillFromStream()
		{
			int oldVirtualSize = _virtualSize;
			if (_virtualSize < _bufferSize) // If the buffer isn't full
			{
				if (_head + _virtualSize < _bufferSize) // If the virtual buffer doesn't wrap
				{
					_virtualSize += _streamIn.Read(_buffer, GetTail(), _bufferSize - GetTail());
					// If we got all the bytes we asked for, then head+_virtualSize now equals _physicalSize.
					// We'll fill in the space at the beginning of the physical buffer with the following Read.
				}

				if (_head + _virtualSize >= _bufferSize) // If the virtual buffer wraps
				{
					_virtualSize += _streamIn.Read(_buffer, GetTail(), _bufferSize - _virtualSize);
				}
			}
			return _virtualSize - oldVirtualSize;
		}

		/// <summary>
		/// This writes up to N bytes to the output stream.  If N is greater than
		/// the number of bytes available, only the available bytes will be written.
		/// </summary>
		/// <param name="streamOut"></param>
		/// <param name="count"></param>
		/// <returns>The number of bytes written.</returns>
		private int WriteToStream(Stream streamOut, int count)
		{
			int oldVirtualSize = _virtualSize;
			if (_virtualSize > 0) // If the buffer isn't empty
			{
				int bytesToWrite;
				if (_head + _virtualSize >= _bufferSize && count > 0) // If the virtual buffer wraps
				{
					bytesToWrite = Min(count, _bufferSize - _head);
					streamOut.Write(_buffer, _head, bytesToWrite);
					count -= bytesToWrite;
					Skip(bytesToWrite); // _head may wrap to zero with this call
				}

				if (_head + _virtualSize < _bufferSize && count > 0) // If the virtual buffer doesn't wrap
				{
					bytesToWrite = Min(count, GetTail() - _head);
					streamOut.Write(_buffer, _head, bytesToWrite);
					Skip(bytesToWrite);
				}
			}
			return oldVirtualSize - _virtualSize;
		}

		// Add two numbers, wrapping at the end of the buffer.
		private int Plus(int i, int j)
		{
			return (i + j) % _bufferSize;
		}

		// Get byte from index i in the virtual buffer.
		private byte GetByte(int i)
		{
			return _buffer[Plus(_head, i)];
		}

		// Get the position in the physical buffer just beyond
		// the end of the virtual buffer.
		private int GetTail()
		{
			return Plus(_head, _virtualSize);
		}

		private int Min(int x, int y)
		{
			return x < y ? x : y;
		}

		// Move the head of the virtual buffer forward.
		private void Skip(int count)
		{
			_head = Plus(_head, count);
			_virtualSize -= count;
		}

		// This uses a naive search algorithm that backtracks after partial matches.
		// Should be fine for our purposes.
		private int IndexOf(byte[] pattern)
		{
			int count = _virtualSize - (pattern.Length - 1);
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; GetByte(i + j) == pattern[j]; j++)
				{
					if (j == pattern.Length-1)
					{
						return i;
					}
				}
			}
			return -1;
		}
		#endregion
	}
}
