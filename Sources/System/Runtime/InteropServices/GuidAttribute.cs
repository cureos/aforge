//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System.Runtime.InteropServices
{
	[AttributeUsageAttribute(
		AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum |
		AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
	public sealed class GuidAttribute : Attribute
	{
		#region CONSTRUCTORS

		public GuidAttribute(string guid)
		{
		}

		#endregion
	}
}