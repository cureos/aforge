using System;
using System.Collections.Generic;

namespace DGP.Util.System{
	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// This class contains a method for generating a simple anonymous 
	/// Comparer without having to create a separate class manually.
	/// <seealso cref="System.IComparer"/>
	/// <seealso cref="System.IComparable"/>
	/// <seealso cref="System.IComparison"/>
	/// </summary>
	///////////////////////////////////////////////////////////////////////////
	public static class ComparerGenerator{
		#region private class implementation
		private class ComparerImplementation<T> : IComparer<T>{
	        private readonly Comparison<T> _comparison;
			
	        public ComparerImplementation(Comparison<T> comparison){
				this._comparison = comparison;
			}
			
	        public int Compare(T x, T y){
				return this._comparison.Invoke(x, y);
			}
	    }
		#endregion
		
		#region public instance methods
		///////////////////////////////////////////////////////////////////////
		/// <summary>
		/// This method is used for generating a simple anonymous 
		/// Comparer without having to create a separate class manually.
		/// </summary>
		/// <returns>The comparer.</returns>
		/// <param name='comparison'>Comparison.</param>
		///////////////////////////////////////////////////////////////////////
		public static IComparer<T> GetComparer<T>(
			Comparison<T> comparison
		){
	        return new ComparerImplementation<T>(comparison);
	    }
		#endregion
	}
}
