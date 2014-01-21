//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System.Runtime.InteropServices
{
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
    public sealed class MarshalAsAttribute : Attribute
	{
		#region CONSTRUCTORS

        public MarshalAsAttribute(UnmanagedType unmanagedType)
        {
        }

        #endregion

        #region FIELDS

        public int SizeConst;

        #endregion
	}
}