//
// System.Data/DataSet.cs
//
// Author:
//   Christopher Podurgiel <cpodurgiel@msn.com>
//   Daniel Morgan <danmorg@sc.rr.com>
//   Rodrigo Moya <rodrigo@ximian.com>
//   Stuart Caborn <stuart.caborn@virgin.net>
//   Tim Coleman (tim@timcoleman.com)
//   Ville Palo <vi64pa@koti.soon.fi>
//   Atsushi Enomoto <atsushi@ximian.com>
//   Konstantin Triger <kostat@mainsoft.com>
//
// (C) Ximian, Inc. 2002
// Copyright (C) Tim Coleman, 2002, 2003
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

using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace System.Data{
	[Serializable]
	public partial class DataSet{
		#region public instance properties
		[DataCategory ("Data")]
		[DefaultValue (false)]
		public bool CaseSensitive {
			get { return caseSensitive; }
			set {
				caseSensitive = value;
				/*
				if (!caseSensitive) {
						foreach (DataTable table in Tables) {
								table.ResetCaseSensitiveIndexes ();
								foreach (Constraint c in table.Constraints)
										c.AssertConstraint ();
						}
				} else {
						foreach (DataTable table in Tables) {
								table.ResetCaseSensitiveIndexes ();
						}
				}
				*/
			}
		}

		[DataCategory ("Data")]
		public CultureInfo Locale {
			get { return locale != null ? locale : Thread.CurrentThread.CurrentCulture; }
			set {
				if (locale == null || !locale.Equals (value)) {
					// TODO: check if the new locale is valid
					// TODO: update locale of all tables
					locale = value;
				}
			}
		}

		[DataCategory ("Data")]
		[DefaultValue ("")]
		public string Namespace {
			get { return _namespace; }
			set {
				//TODO - trigger an event if this happens?
				if (value == null)
					value = String.Empty;
				if (value != this._namespace)
					RaisePropertyChanging ("Namespace");
				_namespace = value;
			}
		}
		#endregion

		#region private instance fields
		private bool caseSensitive;
		private CultureInfo locale;
		private string _namespace = string.Empty;
		#endregion
		
		#region internal instance methods
		[MonoTODO]
		protected internal void RaisePropertyChanging (string name)
		{
		}
		#endregion
	}
}
