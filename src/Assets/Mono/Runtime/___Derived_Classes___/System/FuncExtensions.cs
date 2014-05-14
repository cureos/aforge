namespace System{
	public delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);
	public delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);
	public delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);
	public delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T8 p8);
	public delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T8 p8, T9 p9);
	
	public static class FuncExtensions{}
}
