//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System.Runtime.InteropServices
{
	public struct GCHandle
	{
		#region PROPERTIES

		public bool IsAllocated { get; private set; }
		
		#endregion

		#region METHODS

		public static GCHandle Alloc(object value, GCHandleType type)
		{
			throw new NotImplementedException();
		}

		public IntPtr AddrOfPinnedObject()
		{
			throw new NotImplementedException();
		}

		public void Free()
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}