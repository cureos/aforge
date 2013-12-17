//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

#if NETFX_CORE
	using System.Threading.Tasks;
	using Windows.Storage;
#endif

namespace System.IO
{
	public static class File
	{
        #region METHODS

        public static Stream OpenRead(string path)
        {
#if NETFX_CORE
				return Task.Run(async () =>
				{
					var storageFile = await StorageFile.GetFileFromPathAsync(path);
					var storageStream = await storageFile.OpenSequentialReadAsync();
					return storageStream.AsStreamForRead();
				}).Result;
#else
            throw new NotImplementedException();
#endif
        }
        
        public static Stream OpenWrite(string path)
        {
#if NETFX_CORE
				return Task.Run(async () =>
				{
					var storageFile = await StorageFile.GetFileFromPathAsync(path);
					var storageStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
					return storageStream.AsStreamForWrite();
				}).Result;
#else
            throw new NotImplementedException();
#endif
        }

        #endregion
	}
}