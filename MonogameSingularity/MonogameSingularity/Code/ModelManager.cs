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
		private readonly Dictionary<String, Texture2D> ModelTextureDictionary;


		public ModelManager(ContentManager contentManager)
		{
			if (Instance != null) return;
			Instance = this;

			this.ContentManager = contentManager;
			this.ModelDictionary = new Dictionary<string, Model>();
			this.ModelTextureDictionary = new Dictionary<string, Texture2D>();
		}

		/// <summary>
		/// Gets the singleton instance
		/// </summary>
		/// <returns></returns>
		private static ModelManager GetInstance()
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

			this.ModelTextureDictionary[path] = ((BasicEffect) model.Meshes[0].Effects[0]).Texture;


			return model;
		}

		private Texture2D _GetTexture(String path)
		{
			if (this.ModelTextureDictionary.ContainsKey(path)) return this.ModelTextureDictionary[path];

			_GetModel(path);
			return this.ModelTextureDictionary[path];
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

		public static Texture2D GetTexture(String path)
		{
			return GetInstance()._GetTexture(path);
		}
	}
}
