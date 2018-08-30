using System;
using System.Collections.Generic;

namespace Singularity.Core.Utilities
{
	public class CustomData
	{
		private readonly Dictionary<string, Tuple<Type, object>> DataDictionary;

		public CustomData()
		{
			this.DataDictionary = new Dictionary<string, Tuple<Type, object>>();
		}

		public T GetValue<T>(string key)
		{
			if (!this.DataDictionary.ContainsKey(key)) return default(T);

			if (this.DataDictionary[key].Item1 != typeof(T))
				throw new ArgumentException($"Value of key {key} is not of type {this.DataDictionary[key].Item1}");

			return (T) this.DataDictionary[key].Item2;
		}

		public void SetValue<T>(string key, T value)
		{
			this.DataDictionary[key] = new Tuple<Type, object>(typeof(T), value);
		}

		public bool ContainsKey(string key)
		{
			return this.DataDictionary.ContainsKey(key);
		}
	}
}