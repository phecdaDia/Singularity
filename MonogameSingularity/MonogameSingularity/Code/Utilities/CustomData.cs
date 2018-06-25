using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Utilities
{
	public class CustomData
	{
		private readonly Dictionary<String, Tuple<Type, Object>> DataDictionary;

		public CustomData()
		{
			this.DataDictionary = new Dictionary<string, Tuple<Type, object>>();
		}

		public T GetValue<T>(String key)
		{
			if (!DataDictionary.ContainsKey(key)) throw new ArgumentException($"No value of key {key} found!");

			if (DataDictionary[key].Item1 != typeof(T)) throw new ArgumentException($"Value of key {key} is not of type {DataDictionary[key].Item1}");

			return (T) DataDictionary[key].Item2;
		}

		public void SetValue<T>(String key, T value)
		{
			DataDictionary[key] = new Tuple<Type, object>(typeof(T), value);
		}

		public Boolean ContainsKey(String key)
		{
			return this.DataDictionary.ContainsKey(key);
		}

	}
}
