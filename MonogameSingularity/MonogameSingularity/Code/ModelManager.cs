using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity
{
	public class ModelManager
	{
		private static ModelManager Instance;

		private readonly ContentManager ContentManager;
		private readonly Dictionary<String, Model> ModelDictionary;


		public ModelManager(ContentManager contentManager)
		{
			if (Instance != null) return;
			Instance = this;

			this.ContentManager = contentManager;
			this.ModelDictionary = new Dictionary<string, Model>();
		}

		/// <summary>
		/// Gets the singleton instance
		/// </summary>
		/// <returns></returns>
		public static ModelManager GetInstance()
		{
			return Instance;
		}

		/// <summary>
		/// Gets <seealso cref="Model"/> cached.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private Model _GetModel(String path)
		{
			if (this.ModelDictionary.ContainsKey(path)) return this.ModelDictionary[path];

			Model model = ContentManager.Load<Model>(path);
			this.ModelDictionary[path] = model;

			return model;
		}

		/// <summary>
		/// Gets <seealso cref="Model"/> cached.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Model GetModel(String path)
		{
			return GetInstance()._GetModel(path);
		}
	}
}
