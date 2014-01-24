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

namespace System.Runtime.InteropServices
{
	public static class Marshal
	{
		#region METHODS

		public static IntPtr AllocHGlobal(int cb)
		{
			throw new NotImplementedException();
		}

		public static void FreeHGlobal(IntPtr hglobal)
		{
			throw new NotImplementedException();
		}

		public static void Copy(IntPtr source, byte[] destination, int startIndex, int length)
		{
			throw new NotImplementedException();
		}

		public static void Copy(byte[] source, int startIndex, IntPtr destination, int length)
		{
			throw new NotImplementedException();
		}

		public static int SizeOf(object structure)
		{
			throw new NotImplementedException();
		}

        public static int SizeOf(Type t)
        {
            throw new NotImplementedException();
        }

        public static void StructureToPtr(object structure, IntPtr ptr, bool fDeleteOld)
		{
			throw new NotImplementedException();
		}

		public static object PtrToStructure(IntPtr ptr, Type structureType)
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}