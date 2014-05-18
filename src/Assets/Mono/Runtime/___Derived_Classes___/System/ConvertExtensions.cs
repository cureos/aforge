//
// System.Convert.cs
//
// Authors:
//   Derek Holden (dholden@draper.com)
//   Duncan Mak (duncan@ximian.com)
//   Marek Safar (marek.safar@gmail.com)
//
// (C) Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2013 Xamarin Inc (http://www.xamarin.com)
//
// System.Convert class. This was written word for word off the 
// Library specification for System.Convert in the ECMA TC39 TG2 
// and TG3 working documents. The first page of which has a table
// for all legal conversion scenerios.
//
// This header and the one above it can be formatted however, just trying
// to keep it consistent w/ the existing mcs headers.
//
// This Convert class could be written another way, with each type 
// implementing IConvertible and defining their own conversion functions,
// and this class just calling the type's implementation. Or, they can 
// be defined here and the implementing type can use these functions when 
// defining their IConvertible interface. Byte's ToBoolean() calls 
// Convert.ToBoolean(byte), or Convert.ToBoolean(byte) calls 
// byte.ToBoolean(). The first case is what is done here.
//
// See http://lists.ximian.com/archives/public/mono-list/2001-July/000525.html
//
// There are also conversion functions that are not defined in
// the ECMA draft, such as there is no bool ToBoolean(DateTime value), 
// and placing that somewhere won't compile w/ this Convert since the
// function doesn't exist. However calling that when using Microsoft's
// System.Convert doesn't produce any compiler errors, it just throws
// an InvalidCastException at runtime.
//
// Whenever a decimal, double, or single is converted to an integer
// based type, it is even rounded. This uses Math.Round which only 
// has Round(decimal) and Round(double), so in the Convert from 
// single cases the value is passed to Math as a double. This 
// may not be completely necessary.
//
// The .NET Framework SDK lists DBNull as a member of this class
// as 'public static readonly object DBNull;'. 
//
// It should also be decided if all the cast return values should be
// returned as unchecked or not.
//
// All the XML function comments were auto generated which is why they
// sound someone redundant.
//
// TYPE | BOOL BYTE CHAR DT DEC DBL I16 I32 I64 SBYT SNGL STR UI16 UI32 UI64
// -----+--------------------------------------------------------------------
// BOOL |   X    X           X   X   X   X   X    X    X   X    X    X    X
// BYTE |   X    X    X      X   X   X   X   X    X    X   X    X    X    X
// CHAR |        X    X              X   X   X    X        X    X    X    X
// DT   |                 X                                X
// DEC  |   X    X           X   X   X   X   X    X    X   X    X    X    X
// DBL  |   X    X           X   X   X   X   X    X    X   X    X    X    X
// I16  |   X    X    X      X   X   X   X   X    X    X   X    X    X    X
// I32  |   X    X    X      X   X   X   X   X    X    X   X    X    X    X
// I64  |   X    X    X      X   X   X   X   X    X    X   X    X    X    X
// SBYT |   X    X    X      X   X   X   X   X    X    X   X    X    X    X
// SNGL |   X    X           X   X   X   X   X    X    X   X    X    X    X
// STR  |   X    X    X   X  X   X   X   X   X    X    X   X    X    X    X
// UI16 |   X    X    X      X   X   X   X   X    X    X   X    X    X    X
// UI32 |   X    X    X      X   X   X   X   X    X    X   X    X    X    X
// UI64 |   X    X    X      X   X   X   X   X    X    X   X    X    X    X
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System {
  
//	[CLSCompliant(false)]
	public static class ConvertExtensions {

		// ========== Conversion / Helper Functions ========== //

		public static object ChangeType (object value, Type conversionType)
		{
			if ((value != null) && (conversionType == null))
				throw new ArgumentNullException ("conversionType");
			CultureInfo ci = CultureInfo.CurrentCulture;
			IFormatProvider provider;
			if (conversionType == typeof(DateTime)) {
				provider = ci.DateTimeFormat;
			}
			else {
				provider = ci.NumberFormat;
			}
			return ToType (value, conversionType, provider, true);
		}
		
		public static object ChangeType (object value, TypeCode typeCode)
		{
			CultureInfo ci = CultureInfo.CurrentCulture;
			Type conversionType = conversionTable [(int) typeCode];
			IFormatProvider provider;
			if (conversionType == typeof(DateTime)) {
				provider = ci.DateTimeFormat;
			}
			else {
				provider = ci.NumberFormat;
			}
			return ToType (value, conversionType, provider, true);
		}

		public static object ChangeType (object value, Type conversionType, IFormatProvider provider)
		{
			if ((value != null) && (conversionType == null))
				throw new ArgumentNullException ("conversionType");
			return ToType (value, conversionType, provider, true);
		}
		
		public static object ChangeType (object value, TypeCode typeCode, IFormatProvider provider)
		{
			Type conversionType = conversionTable [(int)typeCode];
			return ToType (value, conversionType, provider, true);
		}


		// Lookup table for the conversion ToType method. Order
		// is important! Used by ToType for comparing the target
		// type, and uses hardcoded array indexes.
		private static readonly Type[] conversionTable = {
			// Valid ICovnertible Types
			null,		    //	0 empty
			typeof (object),   //  1 TypeCode.Object
			typeof (DBNull),   //  2 TypeCode.DBNull
			typeof (Boolean),  //  3 TypeCode.Boolean
			typeof (Char),	   //  4 TypeCode.Char
			typeof (SByte),	   //  5 TypeCode.SByte
			typeof (Byte),	   //  6 TypeCode.Byte
			typeof (Int16),	   //  7 TypeCode.Int16
			typeof (UInt16),   //  8 TypeCode.UInt16
			typeof (Int32),	   //  9 TypeCode.Int32
			typeof (UInt32),   // 10 TypeCode.UInt32
			typeof (Int64),	   // 11 TypeCode.Int64
			typeof (UInt64),   // 12 TypeCode.UInt64
			typeof (Single),   // 13 TypeCode.Single
			typeof (Double),   // 14 TypeCode.Double
			typeof (Decimal),  // 15 TypeCode.Decimal
			typeof (DateTime), // 16 TypeCode.DateTime
			null,		    // 17 null.
			typeof (String),   // 18 TypeCode.String
			typeof (Enum)
		};

		// Function to convert an object to another type and return
		// it as an object. In place for the core data types to use
		// when implementing IConvertible. Uses hardcoded indexes in 
		// the conversionTypes array, so if modify carefully.
	
		//
		// The `try_target_to_type' boolean indicates if the code
		// should try to call the IConvertible.ToType method if everything
		// else fails.
		//
		// This should be true for invocations from Convert.cs, and
		// false from the mscorlib types that implement IConvertible that 
		// all into this internal function.
		//
		// This was added to keep the fix for #481687 working and to avoid
		// the regression that the simple fix introduced (485377)
		internal static object ToType (object value, Type conversionType, IFormatProvider provider, bool try_target_to_type) 
		{
			if (value == null) {
				if ((conversionType != null) && conversionType.IsValueType){
					throw new InvalidCastException ("Null object can not be converted to a value type.");
				} else
					return null;
			}

			if (conversionType == null)
				throw new InvalidCastException ("Cannot cast to destination type.");

			if (value.GetType () == conversionType)
				return value;
			
			IConvertible convertValue = value as IConvertible;
			if (convertValue != null) {

				if (conversionType == conversionTable[0]) // 0 Empty
					throw new ArgumentNullException ();
				
				if (conversionType == conversionTable[1]) // 1 TypeCode.Object
					return value;
					
				if (conversionType == conversionTable[2]) // 2 TypeCode.DBNull
					throw new InvalidCastException (
						"Cannot cast to DBNull, it's not IConvertible");
		  
				if (conversionType == conversionTable[3]) // 3 TypeCode.Boolean
					return convertValue.ToBoolean (provider);
					
				if (conversionType == conversionTable[4]) // 4 TypeCode.Char
					return convertValue.ToChar (provider);
		  
				if (conversionType == conversionTable[5]) // 5 TypeCode.SByte
					return convertValue.ToSByte (provider);

				if (conversionType == conversionTable[6]) // 6 TypeCode.Byte
					return convertValue.ToByte (provider);
				
				if (conversionType == conversionTable[7]) // 7 TypeCode.Int16
					return convertValue.ToInt16 (provider);
					
				if (conversionType == conversionTable[8]) // 8 TypeCode.UInt16
					return convertValue.ToUInt16 (provider);
		  
				if (conversionType == conversionTable[9]) // 9 TypeCode.Int32
					return convertValue.ToInt32 (provider);
			
				if (conversionType == conversionTable[10]) // 10 TypeCode.UInt32
					return convertValue.ToUInt32 (provider);
		  
				if (conversionType == conversionTable[11]) // 11 TypeCode.Int64
					return convertValue.ToInt64 (provider);
		  
				if (conversionType == conversionTable[12]) // 12 TypeCode.UInt64
					return convertValue.ToUInt64 (provider);
		  
				if (conversionType == conversionTable[13]) // 13 TypeCode.Single
					return convertValue.ToSingle (provider);
		  
				if (conversionType == conversionTable[14]) // 14 TypeCode.Double
					return convertValue.ToDouble (provider);

				if (conversionType == conversionTable[15]) // 15 TypeCode.Decimal
					return convertValue.ToDecimal (provider);

				if (conversionType == conversionTable[16]) // 16 TypeCode.DateTime
					return convertValue.ToDateTime (provider);
				
				if (conversionType == conversionTable[18]) // 18 TypeCode.String
					return convertValue.ToString (provider);

				if (conversionType == conversionTable[19] && value is Enum) // System.Enum
					return value;

				if (try_target_to_type)
					return convertValue.ToType (conversionType, provider);
			} 
			// Not in the conversion table
			throw new InvalidCastException (
				"Value is not a convertible object: " + 
				value.GetType().ToString() + 
				" to " + 
				conversionType.FullName
			);
		}
	}
}
