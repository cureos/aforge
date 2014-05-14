using System;
using System.Numerics;
using NUnit.Framework;

namespace MonoTests.System.Numerics{
	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// This class is used for testing the behaviour of the 
	/// <see cref="DGP.Extensions.UnityEngine.Color32Extensions.RgbToHex"> 
	/// method.
	/// </summary>
	///////////////////////////////////////////////////////////////////////////
	[TestFixture]
	public class Test_Complex_ToString{
		///////////////////////////////////////////////////////////////////////
		/// <summary>
		/// This method checks that a ArgumentNullException is returned when
		/// the format parameter has a null value.
		/// </summary>
		///////////////////////////////////////////////////////////////////////
		[Test]
		[Ignore]
		[ExpectedException( typeof(ArgumentNullException) )]
		public void NullFormat_1(){
			new Complex(1.0, 1.0).ToString(null, null);
		}

	}
}

