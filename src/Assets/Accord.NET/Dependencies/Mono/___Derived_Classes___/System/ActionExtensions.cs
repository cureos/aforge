namespace System{
	public delegate void Action<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);
	public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T8 p8);
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T8 p8, T9 p9);
	
	public static class ActionExtensions{
		///////////////////////////////////////////////////////////////////////
		/// <summary>
		/// This method invokes the action passed
		/// as parameter if it isn't null.
		/// </summary>
		/// <param name='action'>Action.</param>
		///////////////////////////////////////////////////////////////////////
		public static bool TryInvoke(this Action action){
			try{
				action();
				return true;
			}catch{
				return false;
			}
		}

		///////////////////////////////////////////////////////////////////////
		/// <summary>
		/// This method invokes the action passed
		/// as parameter if it isn't null.
		/// </summary>
		/// <param name='action'>Action.</param>
		///////////////////////////////////////////////////////////////////////
		public static bool TryInvoke<T1>(this Action<T1> action, T1 p1){
			try{
				action(p1);
				return true;
			}catch{
				return false;
			}
		}

		///////////////////////////////////////////////////////////////////////
		/// <summary>
		/// This method invokes the action passed
		/// as parameter if it isn't null.
		/// </summary>
		/// <param name='action'>Action.</param>
		///////////////////////////////////////////////////////////////////////
		public static bool TryInvoke<T1, T2>(this Action<T1, T2> action, T1 p1, T2 p2){
			try{
				action(p1, p2);
				return true;
			}catch{
				return false;
			}
		}
		
		///////////////////////////////////////////////////////////////////////
		/// <summary>
		/// This method invokes the action passed
		/// as parameter if it isn't null.
		/// </summary>
		/// <param name='action'>Action.</param>
		///////////////////////////////////////////////////////////////////////
		public static bool TryInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3){
			try{
				action(p1, p2, p3);
				return true;
			}catch{
				return false;
			}
		}
	}
}