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