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