//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

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

	    internal FileStream(Stream stream)
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