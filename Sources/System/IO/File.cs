//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
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

        public static FileStream OpenRead(string path)
        {
#if NETFX_CORE
				return Task.Run(async () =>
				{
					var storageFile = await StorageFile.GetFileFromPathAsync(path);
					var storageStream = await storageFile.OpenSequentialReadAsync();
					return new FileStream(storageStream.AsStreamForRead());
				}).Result;
#else
            throw new NotImplementedException();
#endif
        }
        
        public static FileStream OpenWrite(string path)
        {
#if NETFX_CORE
				return Task.Run(async () =>
				{
					var storageFile = await StorageFile.GetFileFromPathAsync(path);
					var storageStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
					return new FileStream(storageStream.AsStreamForWrite());
				}).Result;
#else
            throw new NotImplementedException();
#endif
        }

        #endregion
	}
}