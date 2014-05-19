//
// System.ComponentModel.InvalidEnumArgumentExceptionDerived.cs 
//
// Authors:
//	Duncan Mak (duncan@ximian.com)
//  Andreas Nahr (ClassDevelopment@A-SoftTech.com)
//
// (C) 2002 Ximian, Inc.		http://www.ximian.com
// (C) 2003 Andreas Nahr
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Globalization;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[Serializable]
	public class InvalidEnumArgumentExceptionDerived : ArgumentException
	{

		public InvalidEnumArgumentExceptionDerived () : this ((string) null)
		{
		}

		public InvalidEnumArgumentExceptionDerived (string message) : base (message)
		{
		}

		public InvalidEnumArgumentExceptionDerived (string argumentName, int invalidValue, Type enumClass) :
			base (string.Format (CultureInfo.CurrentCulture, "The value "
					+ "of argument '{0}' ({1}) is invalid for "
					+ "Enum type '{2}'.", argumentName, invalidValue,
					enumClass.Name), argumentName)
		{
		}

		public InvalidEnumArgumentExceptionDerived (string message, Exception innerException)
			: base (message, innerException)
		{
		}
	}
}
