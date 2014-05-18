//
// System.ComponentModel.EventHandlerListDerived.cs
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//   Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// (C) Ximian, Inc.  http://www.ximian.com
//

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
using System.Collections;
using System.Collections.Generic;

namespace System.ComponentModel {

	internal class ListEntryDerived {
		public object key;
		public Delegate value;
		public ListEntryDerived next;
	}

	// <summary>
	//   List of Event delegates.
	// </summary>
	//
	// <remarks>
	//   Longer description
	// </remarks>
	public sealed class EventHandlerListDerived : IDisposable
	{
		ListEntryDerived entries;

		Delegate null_entry;

		public EventHandlerListDerived ()
		{
		}

		public Delegate this [object key] {
			get {
				if (key == null)
					return null_entry;
				ListEntryDerived entry = FindEntry (key);
				if (entry != null)
					return entry.value;
				else
					return null;
			}

			set {
				AddHandler (key, value);
			}
		}

		public void AddHandler (object key, Delegate value)
		{
			if (key == null) {
				null_entry = Delegate.Combine (null_entry, value);
				return;
			}

			ListEntryDerived entry = FindEntry (key);
			if (entry == null) {
				entry = new ListEntryDerived ();
				entry.key = key;
				entry.value = null;
				entry.next = entries;
				entries = entry;
			}

			entry.value = Delegate.Combine (entry.value, value);
		}

		public void AddHandlers (EventHandlerListDerived listToAddFrom)
		{
			if (listToAddFrom == null)
				return;
			
			ListEntryDerived entry = listToAddFrom.entries;
			while (entry != null) {
				AddHandler (entry.key, entry.value);
				entry = entry.next;
			}
		}

		public void RemoveHandler (object key, Delegate value)
		{
			if (key == null) {
				null_entry = Delegate.Remove (null_entry, value);
				return;
			}

			ListEntryDerived entry = FindEntry (key);
			if (entry == null)
				return;

			entry.value = Delegate.Remove (entry.value, value);
		}

		public void Dispose ()
		{
			entries = null;
		}
		
		private ListEntryDerived FindEntry (object key)
		{
			ListEntryDerived entry = entries;
			while (entry != null) {
				if (entry.key == key)
					return entry;
				entry = entry.next;
			}

			return null;
		}
	}
}

