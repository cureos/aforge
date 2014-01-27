/*
 *  Copyright (c) 2013-2014, Cureos AB.
 *  All rights reserved.
 *  http://www.cureos.com
 *
 *	This file is part of Shim.NET.
 *
 *  Shim.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Shim.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Shim.NET.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Threading.Tasks;
using Windows.Storage;

namespace System.IO
{
	public sealed class FileStream : Stream
	{
		#region FIELDS

		private Stream _internalStream;
		private bool _disposed = false;

		#endregion
		
		#region CONSTRUCTORS

		public FileStream(string path, FileMode mode) : this(path, mode, FileAccess.ReadWrite, FileShare.Read)
		{
		}

		public FileStream(string path, FileMode mode, FileAccess access, FileShare share) 
		{
			switch (access)
			{
				case FileAccess.ReadWrite:
				case FileAccess.Write:
					_internalStream = Task.Run(async () =>
					{
						var storageFile = await StorageFile.GetFileFromPathAsync(path);
						return await storageFile.OpenStreamForWriteAsync();
					}).Result;
					break;
				case FileAccess.Read:
					_internalStream = Task.Run(async () =>
					{
						var storageFile = await StorageFile.GetFileFromPathAsync(path);
						return await storageFile.OpenStreamForReadAsync();
					}).Result;
					break;
				default:
					throw new ArgumentException("Unsupported file access type", "access");
			}
		}

		#endregion

		#region PROPERTIES

		public override bool CanRead
		{
			get { return _internalStream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return _internalStream.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return _internalStream.CanWrite; }
		}

		public override long Length
		{
			get { return _internalStream.Length; }
		}

		public override long Position
		{
			get { return _internalStream.Position; }
			set { _internalStream.Position = value; }
		}

		#endregion

		#region METHODS

		public override void Flush()
		{
			_internalStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _internalStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return _internalStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			_internalStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_internalStream.Write(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed) return;

			_internalStream.Dispose();
			_internalStream = null;

			base.Dispose(disposing);
			_disposed = true;
		}

		#endregion
	}
}