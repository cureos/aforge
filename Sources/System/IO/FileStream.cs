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

namespace System.IO
{
	public sealed class FileStream : Stream
	{
		#region CONSTRUCTORS

		public FileStream(string path, FileMode mode)
		{
			throw new NotImplementedException();
		}

		public FileStream(string path, FileMode mode, FileAccess access, FileShare share)
		{
			throw new NotImplementedException();
		}

	    #endregion

		#region PROPERTIES

        public override bool CanRead { get { throw new NotImplementedException(); } }

        public override bool CanSeek { get { throw new NotImplementedException(); } }

        public override bool CanWrite { get { throw new NotImplementedException(); } }
        
        public override long Length { get { throw new NotImplementedException(); } }

        public override long Position { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        
        #endregion

		#region METHODS

        public override void Flush() { throw new NotImplementedException(); }
        
        public override int Read(byte[] buffer, int offset, int count) { throw new NotImplementedException(); }
        
        public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }
        
        public override void SetLength(long value) { throw new NotImplementedException(); }
        
        public override void Write(byte[] buffer, int offset, int count) { throw new NotImplementedException(); }

		#endregion
	}
}