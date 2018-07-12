using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Singularity.Utilities
{
	public class CustomData
	{
		private readonly Dictionary<string, Tuple<Type, object>> DataDictionary;

		public CustomData()
		{
			DataDictionary = new Dictionary<string, Tuple<Type, object>>();
		}

		public T GetValue<T>(string key)
		{
			if (!DataDictionary.ContainsKey(key)) return default(T);

			if (DataDictionary[key].Item1 != typeof(T))
				throw new ArgumentException($"Value of key {key} is not of type {DataDictionary[key].Item1}");

			return (T) DataDictionary[key].Item2;
		}

		public void SetValue<T>(string key, T value)
		{
			DataDictionary[key] = new Tuple<Type, object>(typeof(T), value);
		}

		public bool ContainsKey(string key)
		{
			return DataDictionary.ContainsKey(key);
		}
	}
}