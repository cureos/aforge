using System;

namespace System{
	public class ArrayExtensions{
		public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] input, Converter<TInput, TOutput> converter){
			if (input == null) {
				throw new ArgumentNullException("input");
			}
			if (converter == null) {
				throw new ArgumentNullException("converter");
			}

			TOutput[] output = new TOutput[input.Length];
			for (int i = 0; i < input.Length; ++i){
				output[i] = converter.Invoke(input[i]);
			}
			return output;
		}
	}
}